using Microsoft.Extensions.Options;
using Moq;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.Options;
using PracticalWork.Library.DTO.BookDtos;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.Common;
using PracticalWork.Library.Models.ReaderModels;
using PracticalWork.Library.Services;
using PracticalWork.Tests.Common;

namespace PracticalWork.Library.Tests.Services;

public sealed class LibraryServiceTests
{
    [Fact]
    public async Task BorrowBook_WhenSuccessful_UpdatesRepositoriesPublishesEventAndInvalidatesCache()
    {
        var readerRepository = new Mock<IReaderRepository>();
        var bookRepository = new Mock<IBookRepository>();
        var borrowRepository = new Mock<IBookBorrowRepository>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        var minioService = new Mock<IMinioService>();
        var cacheService = new Mock<ICacheService>();
        var producer = new Mock<IMessageProducer>();
        var service = CreateService(
            readerRepository,
            bookRepository,
            borrowRepository,
            cacheVersionService,
            minioService,
            cacheService,
            producer);

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "DDD",
            Authors = ["Evans"],
            Description = "desc",
            Year = 2003,
            Category = BookCategory.FictionBook,
            Status = BookStatus.Available
        };
        var reader = new Reader { FullName = "John Doe", IsActive = true };
        bookRepository.Setup(x => x.GetBookById(book.Id)).ReturnsAsync(book);
        readerRepository.Setup(x => x.GetReader(It.IsAny<Guid>())).ReturnsAsync(reader);

        var readerId = Guid.NewGuid();
        await service.BorrowBook(book.Id, readerId);

        Assert.Equal(BookStatus.Borrow, book.Status);
        borrowRepository.Verify(x => x.CreateBookBorrow(book.Id, readerId, It.Is<BookBorrow>(b => b.Status == BookIssueStatus.Issued)), Times.Once);
        bookRepository.Verify(x => x.UpdateBook(book, book.Id), Times.Once);
        producer.Verify(x => x.ProduceBookBorrowAsync(It.IsAny<Contracts.Events.BookBorrowedEvent>()), Times.Once);
        VerifyBookCacheInvalidation(cacheVersionService);
    }

    [Fact]
    public async Task BorrowBook_WhenReaderInactive_ThrowsConflictException()
    {
        var readerRepository = new Mock<IReaderRepository>();
        var bookRepository = new Mock<IBookRepository>();
        bookRepository.Setup(x => x.GetBookById(It.IsAny<Guid>())).ReturnsAsync(new Book { Status = BookStatus.Available });
        readerRepository.Setup(x => x.GetReader(It.IsAny<Guid>())).ReturnsAsync(new Reader { IsActive = false });

        var service = CreateService(
            readerRepository,
            bookRepository,
            new Mock<IBookBorrowRepository>(),
            new Mock<ICacheVersionService>(),
            new Mock<IMinioService>(),
            new Mock<ICacheService>(),
            new Mock<IMessageProducer>());

        var exception = await Assert.ThrowsAsync<LibraryServiceException>(() => service.BorrowBook(Guid.NewGuid(), Guid.NewGuid()));

        Assert.Equal("Нельзя выдать книгу с неактивной карточкой", exception.Message);
    }

    [Fact]
    public async Task ReturnBook_WhenSuccessful_UpdatesBorrowAndPublishesEvent()
    {
        var readerRepository = new Mock<IReaderRepository>();
        var bookRepository = new Mock<IBookRepository>();
        var borrowRepository = new Mock<IBookBorrowRepository>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        var producer = new Mock<IMessageProducer>();
        var service = CreateService(
            readerRepository,
            bookRepository,
            borrowRepository,
            cacheVersionService,
            new Mock<IMinioService>(),
            new Mock<ICacheService>(),
            producer,
            utcNow: new DateTime(2026, 5, 20, 10, 0, 0, DateTimeKind.Utc));

        var book = new Book { Title = "Returnable", Status = BookStatus.Borrow };
        var borrow = new BookBorrow
        {
            Book = book,
            DueDate = new DateOnly(2026, 5, 25),
            Status = BookIssueStatus.Issued
        };
        borrowRepository.Setup(x => x.GetBookBorrow(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((Guid.NewGuid(), borrow));
        readerRepository.Setup(x => x.GetReader(It.IsAny<Guid>())).ReturnsAsync(new Reader { FullName = "Reader" });

        await service.ReturnBook(Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal(BookIssueStatus.Returned, borrow.Status);
        Assert.Equal(BookStatus.Available, book.Status);
        borrowRepository.Verify(x => x.UpdateReturnedBookBorrow(It.IsAny<Guid>(), borrow), Times.Once);
        producer.Verify(x => x.ProduceBookReturnAsync(It.IsAny<Contracts.Events.BookReturnedEvent>()), Times.Once);
        VerifyBookCacheInvalidation(cacheVersionService);
    }

    [Fact]
    public async Task GetBookDetails_WhenQueryEmpty_ThrowsException()
    {
        var service = CreateService(
            new Mock<IReaderRepository>(),
            new Mock<IBookRepository>(),
            new Mock<IBookBorrowRepository>(),
            new Mock<ICacheVersionService>(),
            new Mock<IMinioService>(),
            new Mock<ICacheService>(),
            new Mock<IMessageProducer>());

        var exception = await Assert.ThrowsAsync<LibraryServiceException>(() =>
            service.GetBookDetails(new BookDetailsQuery()));

        Assert.Equal("Не указан идентификатор или название книги", exception.Message);
    }

    [Fact]
    public async Task GetBookDetails_WhenCacheHasData_ReturnsCacheWithoutRepositoryOrMinio()
    {
        var cacheVersionService = new Mock<ICacheVersionService>();
        var cacheService = new Mock<ICacheService>();
        cacheVersionService.Setup(x => x.GetVersionAsync("book-details", It.IsAny<CancellationToken>())).ReturnsAsync(3);
        cacheService
            .Setup(x => x.GetAsync<List<BookDetailsDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new BookDetailsDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Cached book",
                    Authors = ["Author"],
                    Description = "desc",
                    Year = 2022,
                    Category = BookCategory.FictionBook,
                    Status = BookStatus.Available,
                    IsArchived = false,
                    CoverImagePath = "cached-url"
                }
            ]);

        var bookRepository = new Mock<IBookRepository>(MockBehavior.Strict);
        var minioService = new Mock<IMinioService>(MockBehavior.Strict);
        var service = CreateService(
            new Mock<IReaderRepository>(),
            bookRepository,
            new Mock<IBookBorrowRepository>(),
            cacheVersionService,
            minioService,
            cacheService,
            new Mock<IMessageProducer>());

        var (bookId, book) = await service.GetBookDetails(Guid.NewGuid());

        Assert.Equal(book.Id, bookId);
        Assert.Equal("Cached book", book.Title);
        bookRepository.Verify(x => x.GetBookById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetBookDetails_WhenBookHasCover_LoadsMinioUrlAndWritesCache()
    {
        var bookRepository = new Mock<IBookRepository>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        var cacheService = new Mock<ICacheService>();
        var minioService = new Mock<IMinioService>();
        cacheVersionService.Setup(x => x.GetVersionAsync("book-details", It.IsAny<CancellationToken>())).ReturnsAsync(1);
        cacheService
            .Setup(x => x.GetAsync<List<BookDetailsDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<List<BookDetailsDto>>(null!));

        var bookId = Guid.NewGuid();
        bookRepository.Setup(x => x.GetBookById(bookId)).ReturnsAsync(new Book
        {
            Id = bookId,
            Title = "Book",
            Authors = ["A"],
            Description = "desc",
            Year = 2020,
            Category = BookCategory.FictionBook,
            Status = BookStatus.Available,
            CoverImagePath = "cover-object"
        });
        minioService.Setup(x => x.GetFileUrlAsync("covers", "cover-object", It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://minio/cover-object");

        var service = CreateService(
            new Mock<IReaderRepository>(),
            bookRepository,
            new Mock<IBookBorrowRepository>(),
            cacheVersionService,
            minioService,
            cacheService,
            new Mock<IMessageProducer>());

        var (_, result) = await service.GetBookDetails(bookId);

        Assert.Equal("https://minio/cover-object", result.CoverImagePath);
        cacheService.Verify(x => x.SetAsync(
            It.IsAny<string>(),
            It.Is<List<BookDetailsDto>>(items => items.Count == 1 && items[0].CoverImagePath == "https://minio/cover-object"),
            It.IsAny<TimeSpan>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static LibraryService CreateService(
        Mock<IReaderRepository> readerRepository,
        Mock<IBookRepository> bookRepository,
        Mock<IBookBorrowRepository> borrowRepository,
        Mock<ICacheVersionService> cacheVersionService,
        Mock<IMinioService> minioService,
        Mock<ICacheService> cacheService,
        Mock<IMessageProducer> producer,
        DateTime? utcNow = null)
    {
        return new LibraryService(
            readerRepository.Object,
            bookRepository.Object,
            borrowRepository.Object,
            cacheVersionService.Object,
            minioService.Object,
            cacheService.Object,
            producer.Object,
            new StaticOptionsMonitor<BooksCacheOptions>(new BooksCacheOptions
            {
                BooksListCacheOptions = new CacheOptions { Prefix = "books-list", TtlMinutes = 10 },
                LibraryBooksCacheOptions = new CacheOptions { Prefix = "library-books", TtlMinutes = 10 },
                BookDetailsCacheOptions = new CacheOptions { Prefix = "book-details", TtlMinutes = 10 }
            }),
            new StaticOptionsMonitor<MinioOptions>(new MinioOptions { CoversBucketName = "covers" }),
            new TestTimeProvider(utcNow ?? new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));
    }

    private static void VerifyBookCacheInvalidation(Mock<ICacheVersionService> cacheVersionService)
    {
        cacheVersionService.Verify(x => x.IncrementVersionAsync("books-list", It.IsAny<CancellationToken>()), Times.Once);
        cacheVersionService.Verify(x => x.IncrementVersionAsync("library-books", It.IsAny<CancellationToken>()), Times.Once);
        cacheVersionService.Verify(x => x.IncrementVersionAsync("book-details", It.IsAny<CancellationToken>()), Times.Once);
    }
}
