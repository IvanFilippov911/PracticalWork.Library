using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.Contracts.Helpers;
using PracticalWork.Library.Contracts.Options;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.DTO.BookDtos;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.Common;
using ContractCacheService = PracticalWork.Library.Contracts.Abstractions.Services.ICacheService;
using ContractCacheVersionService = PracticalWork.Library.Contracts.Abstractions.Services.ICacheVersionService;
using ContractMinioService = PracticalWork.Library.Contracts.Abstractions.Services.IMinioService;

namespace PracticalWork.Library.Services;

public sealed class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ContractCacheService _cacheService;
    private readonly ContractCacheVersionService _cacheVersionService;
    private readonly BooksCacheOptions _cacheOptions;
    private readonly ContractMinioService _minioService;
    private readonly MinioOptions _minioOptions;
    private readonly IMessageProducer _producer;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="bookRepository">Репозиторий работы с книгами</param>
    /// <param name="cacheService">Сервис кеша</param>
    /// <param name="minioService">Сервис минио</param>
    /// <param name="cacheVersionService">Сервис версионирования кеша</param>
    /// <param name="cacheOptions">Опции кеша для книг</param>
    /// <param name="minioOptions">опции для минио</param>
    /// <param name="producer">продюсер</param>
    /// <param name="timeProvider">Поставщик времени</param>
    public BookService(
        IBookRepository bookRepository,
        ContractCacheService cacheService,
        ContractMinioService minioService,
        ContractCacheVersionService cacheVersionService,
        IOptionsMonitor<BooksCacheOptions> cacheOptions,
        IOptionsMonitor<MinioOptions> minioOptions,
        IMessageProducer producer,
        TimeProvider timeProvider)
    {
        _minioService = minioService;
        _bookRepository = bookRepository;
        _cacheService = cacheService;
        _cacheVersionService = cacheVersionService;
        _producer = producer;
        _cacheOptions = cacheOptions.CurrentValue;
        _minioOptions = minioOptions.CurrentValue;
        _timeProvider = timeProvider;
    }
    
    /// <inheritdoc/> 
    public async Task<Guid> CreateBook(Book book)
    {
        book.Status = BookStatus.Available;
        try
        {
            var bookId = await _bookRepository.CreateBook(book);
            var message = new BookCreatedEvent(
                bookId, book.Title,
                book.Category.ToString(),
                book.Authors.ToArray(),
                book.Year);
            await _producer.ProduceBookCreateAsync(message);
            await CacheManager.InvalidateBookCacheAsync(_cacheVersionService, _cacheOptions);
            return bookId;
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Ошибка создание книги!", ex);
        }
    }

    /// <inheritdoc/> 
    public async Task UpdateBook(Book model, Guid id)
    {
        var book = await _bookRepository.GetBookById(id);

        if (book.IsArchived)
        {
            throw new BookServiceException("Книга в архиве");
        }

        book.Update(model);
        
        await _bookRepository.UpdateBook(book, id);
        
        await CacheManager.InvalidateBookCacheAsync(_cacheVersionService,_cacheOptions);
    }

    /// <inheritdoc/> 
    public async Task<BookArchive> ArchiveBook(Guid id)
    {
       var book = await _bookRepository.GetBookById(id);
       book.Archive();
       
       await _bookRepository.UpdateBook(book, id);
       var response = new BookArchive
       {
           Id = id,
           Title = book.Title,
           ArchivedAt = _timeProvider.GetUtcNow().UtcDateTime
       };
       var message = new BookArchivedEvent(id, book.Title, 
           "Вызван метод архивации книги", response.ArchivedAt);
       await _producer.ProduceBookArchiveAsync(message);
       await CacheManager.InvalidateBookCacheAsync(_cacheVersionService,_cacheOptions);
       
       return response;
    }

    /// <inheritdoc />
    public async Task<PagedResult<Book>> GetBooks(BookSearchCriteria requestModel)
    {
        var cacheCheckResult = await CacheManager.CheckCacheAsync<Book,BookListDto>(
            _cacheVersionService,_cacheService,
            _cacheOptions.BooksListCacheOptions.Prefix, requestModel,
            dto => new Book
            {
                Id = dto.Id,
                Title = dto.Title,
                Authors = dto.Authors,
                Description = dto.Description,
                Year = dto.Year,
                Category = dto.Category,
                Status = dto.Status,
                IsArchived = dto.IsArchived
            });
        if (cacheCheckResult.Count != 0)
        {
            return new PagedResult<Book>
            {
                Entities = cacheCheckResult,
                TotalCount = cacheCheckResult.Count,
                PageNumber = requestModel.PageNumber,
                PageSize = requestModel.PageSize,
            };
        }
        var booksFromDb = await _bookRepository.GetBooks(requestModel);

        await CacheManager.WriteToCacheAsync(
            _cacheVersionService, _cacheService,
            _cacheOptions.BooksListCacheOptions, requestModel, booksFromDb.Item1,
            book => new BookListDto
            {
                Id = book.Id,
                Title = book.Title,
                Authors = book.Authors,
                Description = book.Description,
                Year = book.Year,
                Category = book.Category,
                Status = book.Status,
                IsArchived = book.IsArchived
            });
        return new PagedResult<Book>
        {
            Entities = booksFromDb.Item1,
            TotalCount = booksFromDb.Item2,
            PageNumber = requestModel.PageNumber,
            PageSize = requestModel.PageSize,
        };
    }
    
    
    public async Task UpdateBookDetails(Guid id, string description, BookCoverUpload coverFile)
    {
        var book = await _bookRepository.GetBookById(id);

        if (book.IsArchived)
            throw new BookServiceException("Нельзя изменить архивную книгу");

        var coverImagePath = book.CoverImagePath;

        if (coverFile.HasFile)
        {
            ValidateCover(coverFile);
            coverImagePath = BuildCoverFilePath(id);
            await _minioService.UploadFileAsync(
                _minioOptions.CoversBucketName,
                coverImagePath,
                coverFile.Stream!,
                coverFile.ContentType!);
        }

        book.UpdateDetails(description, coverImagePath);

        await _bookRepository.UpdateBook(book, id);

        await CacheManager.InvalidateBookCacheAsync(_cacheVersionService,_cacheOptions);
    }

    private string BuildCoverFilePath(Guid bookId)
    {
        var currentDate = _timeProvider.GetUtcNow().UtcDateTime;
        return $"book-covers/{currentDate.Year}/{currentDate.Month}/{bookId}.";
    }

    private void ValidateCover(BookCoverUpload file)
    {
        if (file.Stream is null)
            throw new BookServiceException("Файл обложки не передан");

        if (file.Stream.Length > 5 * 1024 * 1024)
            throw new BookServiceException("Размер обложки превышает 5 МБ");

        if (file.ContentType is not ("image/jpeg" or "image/png"))
            throw new BookServiceException("Разрешён только JPEG или PNG");
    }
}
