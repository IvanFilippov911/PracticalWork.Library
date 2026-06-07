using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Data.PostgreSql.Extensions;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.Common;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий <see cref="AbstractBookEntity"/>
/// </summary>
public sealed class BookRepository : IBookRepository
{
    private readonly AppDbContext _appDbContext;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="appDbContext">Контекст БД</param>
    /// <param name="timeProvider">Поставщик времени</param>
    public BookRepository(AppDbContext appDbContext, TimeProvider timeProvider)
    {
        _appDbContext = appDbContext;
        _timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateBook(Book book)
    {
        AbstractBookEntity entity = book.Category switch
        {
            BookCategory.ScientificBook => new ScientificBookEntity(),
            BookCategory.EducationalBook => new EducationalBookEntity(),
            BookCategory.FictionBook => new FictionBookEntity(),
            _ => throw new ArgumentException($"Неподдерживаемая категория книги: {book.Category}", nameof(book.Category))
        };

        entity.Title = book.Title;
        entity.Description = book.Description;
        entity.Year = book.Year;
        entity.Authors = book.Authors;
        entity.Status = book.Status;
        entity.Category = book.Category;
        
        _appDbContext.Add(entity);
        await _appDbContext.SaveChangesAsync();

        return entity.Id;
    }

    /// <inheritdoc />
    public async Task UpdateBook(Book book, Guid bookId)
    {
        var bookEntity = await _appDbContext.Books.FindAsync(bookId)
            ?? throw new EntityNotFoundException<AbstractBookEntity>(bookId);
        
        bookEntity.Title = book.Title;
        bookEntity.Description = book.Description;
        bookEntity.Authors = book.Authors;
        bookEntity.Status = book.Status;
        bookEntity.Year = book.Year;
        bookEntity.CoverImagePath = book.CoverImagePath;
        bookEntity.UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime;
        
        await _appDbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<Book> GetBookById(Guid id)
    {
        var bookEntity = await _appDbContext.Books.FindAsync(id)
            ?? throw new EntityNotFoundException<AbstractBookEntity>(id);

        return bookEntity.ToBook();
    }
    
    
    /// <inheritdoc />
    public async Task<Guid> GetBookIdByTitle(string title)
    {
        var bookEntity = await _appDbContext.Books
            .FirstOrDefaultAsync(b => b.Title == title);
        return bookEntity.Id;
    }
    
    /// <inheritdoc />
    public async Task<(IReadOnlyList<Book>, int)> GetNonArchivedBooksPageWithIssuanceRecords(
        PageRequest request)
    {
        var query = _appDbContext.Books
            .Where(b => b.Status != BookStatus.Archived)
            .Include(b => b.IssuanceRecords);
        
        var entities = query
            .SkipTake(request)
            .Select(e => e.ToBook());
        
        return (await entities.ToListAsync(), await query.CountAsync());
    }
    
    /// <inheritdoc />
    public async Task<(IReadOnlyList<Book>, int)> GetBooks(BookSearchCriteria request)
    {
        IQueryable<AbstractBookEntity> query = request.Category switch
        {
            BookCategory.ScientificBook => _appDbContext.ScientificBooks,
            BookCategory.EducationalBook => _appDbContext.EducationalBooks,
            BookCategory.FictionBook => _appDbContext.FictionBooks,
            _ => _appDbContext.Books};
        
        var entities = await query
            .Where(x => !request.Status.HasValue || x.Status == request.Status)
            .Where(x => !request.Category.HasValue || x.Category == request.Category)
            .Where(x => string.IsNullOrWhiteSpace(request.Author) || x.Authors.Contains(request.Author))
            .SkipTake(request)
            .Select(x => x.ToBook())
            .ToListAsync();
        
        return (entities, await query.CountAsync());
    }

}
