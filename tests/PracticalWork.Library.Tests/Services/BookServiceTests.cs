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
using PracticalWork.Library.Services;
using PracticalWork.Tests.Common;

namespace PracticalWork.Library.Tests.Services;

public sealed class BookServiceTests
{
    [Fact]
    public async Task CreateBook_WhenSuccessful_SetsStatusPublishesEventAndInvalidatesCache()
    {
        var repository = new Mock<IBookRepository>();
        var cacheService = new Mock<ICacheService>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        var minioService = new Mock<IMinioService>();
        var producer = new Mock<IMessageProducer>();
        var service = CreateService(
            repository,
            cacheService,
            cacheVersionService,
            minioService,
            producer);

        var expectedId = Guid.NewGuid();
        repository.Setup(x => x.CreateBook(It.IsAny<Book>())).ReturnsAsync(expectedId);

        var book = CreateBook();

        var result = await service.CreateBook(book);

        Assert.Equal(expectedId, result);
        Assert.Equal(BookStatus.Available, book.Status);
        repository.Verify(x => x.CreateBook(book), Times.Once);
        producer.Verify(x => x.ProduceBookCreateAsync(It.IsAny<Contracts.Events.BookCreatedEvent>()), Times.Once);
        VerifyBookCacheInvalidation(cacheVersionService);
    }

    [Fact]
    public async Task CreateBook_WhenRepositoryFails_WrapsException()
    {
        var repository = new Mock<IBookRepository>();
        var service = CreateService(
            repository,
            new Mock<ICacheService>(),
            new Mock<ICacheVersionService>(),
            new Mock<IMinioService>(),
            new Mock<IMessageProducer>());

        repository
            .Setup(x => x.CreateBook(It.IsAny<Book>()))
            .ThrowsAsync(new InvalidOperationException("db failure"));

        var exception = await Assert.ThrowsAsync<BookServiceException>(() => service.CreateBook(CreateBook()));

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public async Task UpdateBook_WhenBookArchived_ThrowsConflictException()
    {
        var repository = new Mock<IBookRepository>();
        repository.Setup(x => x.GetBookById(It.IsAny<Guid>()))
            .ReturnsAsync(new Book { IsArchived = true, Authors = ["A"], Title = "T", Description = "D" });

        var service = CreateService(
            repository,
            new Mock<ICacheService>(),
            new Mock<ICacheVersionService>(),
            new Mock<IMinioService>(),
            new Mock<IMessageProducer>());

        var exception = await Assert.ThrowsAsync<BookServiceException>(() => service.UpdateBook(CreateBook(), Guid.NewGuid()));

        Assert.Equal("Книга в архиве", exception.Message);
        repository.Verify(x => x.UpdateBook(It.IsAny<Book>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetBooks_WhenCacheHasData_ReturnsCachedPageWithoutRepositoryCall()
    {
        var repository = new Mock<IBookRepository>(MockBehavior.Strict);
        var cacheService = new Mock<ICacheService>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        cacheVersionService.Setup(x => x.GetVersionAsync("books-list", It.IsAny<CancellationToken>())).ReturnsAsync(5);
        cacheService
            .Setup(x => x.GetAsync<List<BookListDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new BookListDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Cached",
                    Authors = ["Author"],
                    Description = "desc",
                    Year = 2020,
                    Category = BookCategory.FictionBook,
                    Status = BookStatus.Available,
                    IsArchived = false
                }
            ]);

        var service = CreateService(
            repository,
            cacheService,
            cacheVersionService,
            new Mock<IMinioService>(),
            new Mock<IMessageProducer>());

        var result = await service.GetBooks(new BookSearchCriteria { PageNumber = 2, PageSize = 5 });

        Assert.Single(result.Entities);
        Assert.Equal("Cached", result.Entities[0].Title);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        repository.Verify(x => x.GetBooks(It.IsAny<BookSearchCriteria>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBookDetails_WhenCoverTooLarge_ThrowsAndDoesNotUpload()
    {
        var repository = new Mock<IBookRepository>();
        var minioService = new Mock<IMinioService>();
        repository.Setup(x => x.GetBookById(It.IsAny<Guid>())).ReturnsAsync(CreateBook());

        var service = CreateService(
            repository,
            new Mock<ICacheService>(),
            new Mock<ICacheVersionService>(),
            minioService,
            new Mock<IMessageProducer>());

        using var stream = new MemoryStream(new byte[5 * 1024 * 1024 + 1]);
        var upload = new BookCoverUpload("cover.png", "image/png", stream);

        var exception = await Assert.ThrowsAsync<BookServiceException>(() =>
            service.UpdateBookDetails(Guid.NewGuid(), "Updated", upload));

        Assert.Equal("Размер обложки превышает 5 МБ", exception.Message);
        minioService.Verify(
            x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateBookDetails_WhenCoverValid_UploadsAndInvalidatesCache()
    {
        var repository = new Mock<IBookRepository>();
        var minioService = new Mock<IMinioService>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        var book = CreateBook();
        repository.Setup(x => x.GetBookById(It.IsAny<Guid>())).ReturnsAsync(book);

        var service = CreateService(
            repository,
            new Mock<ICacheService>(),
            cacheVersionService,
            minioService,
            new Mock<IMessageProducer>(),
            utcNow: new DateTime(2026, 5, 26, 9, 30, 0, DateTimeKind.Utc));

        var bookId = Guid.NewGuid();
        using var stream = new MemoryStream(new byte[128]);
        var upload = new BookCoverUpload("cover.png", "image/png", stream);

        await service.UpdateBookDetails(bookId, "Updated", upload);

        minioService.Verify(
            x => x.UploadFileAsync(
                "covers",
                $"book-covers/2026/5/{bookId}.",
                stream,
                "image/png",
                It.IsAny<CancellationToken>()),
            Times.Once);
        repository.Verify(x => x.UpdateBook(
            It.Is<Book>(b => b.Description == "Updated" && b.CoverImagePath == $"book-covers/2026/5/{bookId}."),
            bookId), Times.Once);
        VerifyBookCacheInvalidation(cacheVersionService);
    }

    private static BookService CreateService(
        Mock<IBookRepository> repository,
        Mock<ICacheService> cacheService,
        Mock<ICacheVersionService> cacheVersionService,
        Mock<IMinioService> minioService,
        Mock<IMessageProducer> producer,
        DateTime? utcNow = null)
    {
        return new BookService(
            repository.Object,
            cacheService.Object,
            minioService.Object,
            cacheVersionService.Object,
            new StaticOptionsMonitor<BooksCacheOptions>(CreateCacheOptions()),
            new StaticOptionsMonitor<MinioOptions>(new MinioOptions { CoversBucketName = "covers" }),
            producer.Object,
            new TestTimeProvider(utcNow ?? new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));
    }

    private static BooksCacheOptions CreateCacheOptions() => new()
    {
        BooksListCacheOptions = new CacheOptions { Prefix = "books-list", TtlMinutes = 10 },
        BookDetailsCacheOptions = new CacheOptions { Prefix = "book-details", TtlMinutes = 10 },
        LibraryBooksCacheOptions = new CacheOptions { Prefix = "library-books", TtlMinutes = 10 }
    };

    private static Book CreateBook() => new()
    {
        Id = Guid.NewGuid(),
        Title = "Book",
        Authors = ["Author"],
        Description = "Description",
        Year = 2024,
        Category = BookCategory.FictionBook,
        Status = BookStatus.Available,
        IsArchived = false
    };

    private static void VerifyBookCacheInvalidation(Mock<ICacheVersionService> cacheVersionService)
    {
        cacheVersionService.Verify(x => x.IncrementVersionAsync("books-list", It.IsAny<CancellationToken>()), Times.Once);
        cacheVersionService.Verify(x => x.IncrementVersionAsync("library-books", It.IsAny<CancellationToken>()), Times.Once);
        cacheVersionService.Verify(x => x.IncrementVersionAsync("book-details", It.IsAny<CancellationToken>()), Times.Once);
    }
}
