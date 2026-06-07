using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.Contracts.Helpers;
using PracticalWork.Library.Contracts.Options;
using PracticalWork.Library.DTO.BookDtos;
using PracticalWork.Library.DTO.LibraryDtos;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.Common;
using ContractCacheService = PracticalWork.Library.Contracts.Abstractions.Services.ICacheService;
using ContractCacheVersionService = PracticalWork.Library.Contracts.Abstractions.Services.ICacheVersionService;
using ContractMinioService = PracticalWork.Library.Contracts.Abstractions.Services.IMinioService;

namespace PracticalWork.Library.Services;

/// <summary>
/// Сервис сценариев выдачи, возврата и просмотра книг библиотеки.
/// </summary>
public class LibraryService: ILibraryService
{
    private readonly IReaderRepository _readerRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IBookBorrowRepository _bookBorrowRepository;
    private readonly ContractMinioService _minioService;
    private readonly ContractCacheService _cacheService;
    private readonly ContractCacheVersionService _cacheVersionService;
    private readonly IMessageProducer _producer;
    private readonly BooksCacheOptions _cacheOptions;
    private readonly MinioOptions _minioOptions;
    private readonly TimeProvider _timeProvider;
    
    /// <summary>
    /// Инициализирует сервис библиотечных сценариев.
    /// </summary>
    /// <param name="readerRepository">Репозиторий читателей.</param>
    /// <param name="bookRepository">Репозиторий книг.</param>
    /// <param name="bookBorrowRepository">Репозиторий выдач книг.</param>
    /// <param name="cacheVersionService">Сервис версионирования кэша.</param>
    /// <param name="minioService">Сервис файлового хранилища.</param>
    /// <param name="cacheService">Сервис распределенного кэша.</param>
    /// <param name="producer">Продюсер сообщений.</param>
    /// <param name="cacheOptions">Опции кэша.</param>
    /// <param name="minioOptions">Опции MinIO.</param>
    /// <param name="timeProvider">Поставщик времени.</param>
    public LibraryService(IReaderRepository readerRepository, 
        IBookRepository bookRepository,
        IBookBorrowRepository bookBorrowRepository,
        ContractCacheVersionService cacheVersionService,
        ContractMinioService minioService,
        ContractCacheService cacheService,
        IMessageProducer producer,
        IOptionsMonitor<BooksCacheOptions> cacheOptions,
        IOptionsMonitor<MinioOptions> minioOptions,
        TimeProvider timeProvider)
    {
        _readerRepository = readerRepository;
        _bookRepository = bookRepository;
        _bookBorrowRepository = bookBorrowRepository;
        _minioService = minioService;
        _cacheService = cacheService;
        _cacheVersionService = cacheVersionService;
        _cacheOptions = cacheOptions.CurrentValue;
        _minioOptions = minioOptions.CurrentValue;
        _producer = producer;
        _timeProvider = timeProvider;
    }
    
    /// <summary>
    /// Выдает книгу читателю.
    /// </summary>
    /// <param name="bookId">Идентификатор книги.</param>
    /// <param name="readerId">Идентификатор читателя.</param>
    public async Task BorrowBook(Guid bookId, Guid readerId)
    {
        var book = await _bookRepository.GetBookById(bookId);
        var reader = await _readerRepository.GetReader(readerId);
        if (book.Status is BookStatus.Borrow or BookStatus.Archived)
        {
            throw new LibraryServiceException("Нельзя выдать архивную или выданную книгу");
        }

        if (!reader.IsActive)
        {
            throw new LibraryServiceException("Нельзя выдать книгу с неактивной карточкой");
        }
        var bookBorrow = BookBorrow.CreateBookBorrow(_timeProvider);
        book.Status = BookStatus.Borrow;
        await _bookBorrowRepository.CreateBookBorrow(bookId, readerId, bookBorrow);
        await _bookRepository.UpdateBook(book, bookId);
        var message = new BookBorrowedEvent(bookId, readerId, book.Title, 
            reader.FullName, bookBorrow.BorrowDate, bookBorrow.DueDate );
        await _producer.ProduceBookBorrowAsync(message);
        await CacheManager.InvalidateBookCacheAsync(_cacheVersionService,_cacheOptions);
    }

    /// <summary>
    /// Оформляет возврат книги читателем.
    /// </summary>
    /// <param name="bookId">Идентификатор книги.</param>
    /// <param name="readerId">Идентификатор читателя.</param>
    public async Task ReturnBook(Guid bookId, Guid readerId)
    {
        var (id, bookBorrow) = await _bookBorrowRepository.GetBookBorrow(bookId, readerId);
        var reader = await _readerRepository.GetReader(readerId);
        if (bookBorrow.Status != BookIssueStatus.Issued)
        {
            throw new LibraryServiceException("Книга уже возвращена");
        }
        bookBorrow.ReturnBookBorrow(_timeProvider);
        await _bookBorrowRepository.UpdateReturnedBookBorrow(id, bookBorrow);
        var message = new BookReturnedEvent(bookId, readerId, bookBorrow.Book.Title, 
            reader.FullName, bookBorrow.ReturnDate);
        await _producer.ProduceBookReturnAsync(message);
        await CacheManager.InvalidateBookCacheAsync(_cacheVersionService,_cacheOptions);
    }

    /// <summary>
    /// Возвращает подробную информацию о книге по идентификатору.
    /// </summary>
    /// <param name="bookId">Идентификатор книги.</param>
    /// <returns>Идентификатор книги и ее модель.</returns>
    public async Task<(Guid bookId, Book book)> GetBookDetails(Guid bookId)
    {
        var cacheCheckResult = await CacheManager.CheckCacheAsync<Book,BookDetailsDto>(
            _cacheVersionService,_cacheService,
            _cacheOptions.BookDetailsCacheOptions.Prefix, bookId,
            dto => new Book
            {
                Title = dto.Title,
                Authors = dto.Authors,
                Category = dto.Category,
                CoverImagePath = dto.CoverImagePath,
                Description = dto.Description,
                Id = dto.Id,
                IsArchived = dto.IsArchived,
                Year = dto.Year,
                Status = dto.Status,
            });
        if (cacheCheckResult.Count != 0)
        {
            var book = cacheCheckResult[0];
            return (book.Id, book);
        }
        var bookFromDb = await _bookRepository.GetBookById(bookId);
        if (bookFromDb.CoverImagePath is not null)
        {
            try
            {
                bookFromDb.CoverImagePath = await _minioService.GetFileUrlAsync(_minioOptions.CoversBucketName, bookFromDb.CoverImagePath);
            }
            catch (Exception)
            {
                throw new LibraryServiceException("Путь к обложке книги невалидный");
            }
        }
        await CacheManager.WriteToCacheAsync(
            _cacheVersionService, _cacheService,
            _cacheOptions.BookDetailsCacheOptions, bookId, [bookFromDb],
            b => new BookDetailsDto
            {
                Id = b.Id,
                Title = b.Title,
                Year = b.Year,
                Description = b.Description,
                Status = b.Status,
                IsArchived = b.IsArchived,
                Authors = b.Authors,
                CoverImagePath = b.CoverImagePath,
                Category = b.Category,
            });
        return (bookId, bookFromDb);
    }

    /// <summary>
    /// Возвращает подробную информацию о книге по составному запросу.
    /// </summary>
    /// <param name="query">Запрос на получение книги по идентификатору или названию.</param>
    /// <returns>Идентификатор книги и ее модель.</returns>
    public async Task<(Guid bookId, Book book)> GetBookDetails(BookDetailsQuery query)
    {
        if (query.BookId.HasValue)
        {
            return await GetBookDetails(query.BookId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            return await GetBookDetails(query.Title);
        }

        throw new LibraryServiceException("Не указан идентификатор или название книги");
    }

    /// <summary>
    /// Возвращает подробную информацию о книге по названию.
    /// </summary>
    /// <param name="title">Название книги.</param>
    /// <returns>Идентификатор книги и ее модель.</returns>
    public async Task<(Guid bookId, Book book)> GetBookDetails(string title)
    {
        var id = await _bookRepository.GetBookIdByTitle(title);
        return await GetBookDetails(id);
    }

    /// <summary>
    /// Возвращает страницу неархивированных книг вместе с историей выдач.
    /// </summary>
    /// <param name="request">Параметры пагинации.</param>
    /// <returns>Страница книг.</returns>
    public async Task<PagedResult<Book>> GetNonArchivedBooksPage(PageRequest request)
    {
        
        var checkCacheResult = await CacheManager.CheckCacheAsync<Book,LibraryBookDto>(
            _cacheVersionService,
            _cacheService,
            _cacheOptions.LibraryBooksCacheOptions.Prefix,
            request,
            dto => new Book
            {
                Title = dto.Title,
                Authors = dto.Authors,
                Description = dto.Description,
                Year = dto.Year,
                Category = dto.Category,
                Status = dto.Status,
                IsArchived = dto.IsArchived,
                IssuanceRecords = dto.IssuanceRecords.Select(i => new BookBorrow
                    {
                        BorrowDate = i.BorrowDate,
                        ReturnDate = i.ReturnDate,
                        DueDate = i.DueDate,
                        Status = i.Status,
                    }).ToList()
                
            });
        if (checkCacheResult.Count != 0)
        {
            return new PagedResult<Book>
            {
                Entities = checkCacheResult,
                TotalCount = checkCacheResult.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }
        
        var booksFromDb = await _bookRepository
            .GetNonArchivedBooksPageWithIssuanceRecords(request);
        
        await CacheManager.WriteToCacheAsync(_cacheVersionService, _cacheService,
            _cacheOptions.LibraryBooksCacheOptions, request, booksFromDb.Item1,
            book => new LibraryBookDto
            {
                Title = book.Title,
                Authors = book.Authors,
                Description = book.Description,
                Year = book.Year,
                Category = book.Category,
                Status = book.Status,
                IsArchived = book.IsArchived,
                IssuanceRecords = book.IssuanceRecords.Select(i => new IssuanceDto
                {
                    BorrowDate = i.BorrowDate,
                    DueDate = i.DueDate,
                    ReturnDate = i.ReturnDate,
                    Status = i.Status,
                }).ToList()
            });
        return new PagedResult<Book>
        {
            Entities = booksFromDb.Item1,
            TotalCount = booksFromDb.Item2,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
        };
    }
}
