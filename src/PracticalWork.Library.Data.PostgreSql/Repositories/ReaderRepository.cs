using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Data.PostgreSql.Extensions;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.ReaderModels;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

public class ReaderRepository: IReaderRepository
{
    private readonly AppDbContext _appDbContext;
    private readonly TimeProvider _timeProvider;

    public ReaderRepository(AppDbContext context, TimeProvider timeProvider)
    {
        _appDbContext = context;
        _timeProvider = timeProvider;
    }
    /// <inheritdoc />
    public async Task<Guid> CreateReader(Reader reader)
    {
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        ReaderEntity readerEntity = new()
        {
            FullName = reader.FullName,
            PhoneNumber = reader.PhoneNumber,
            Email = reader.Email,
            ExpiryDate = reader.ExpiryDate,
            IsActive = reader.IsActive,
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };
        _appDbContext.Readers.Add(readerEntity);
        await _appDbContext.SaveChangesAsync();
        
        return readerEntity.Id;
    }
    /// <inheritdoc />
    public async Task<bool> IsExistReader(string phone)
    {
        return await _appDbContext.Readers.AnyAsync(reader => reader.PhoneNumber == phone);
    }
    /// <inheritdoc />
    public async Task<Reader> GetReader(Guid id)
    {
        var reader = await _appDbContext.Readers
            .SingleOrDefaultAsync(reader => reader.Id == id) ?? throw new EntityNotFoundException<Reader>(id);
        return new Reader
        {
            FullName = reader.FullName,
            ExpiryDate = reader.ExpiryDate,
            IsActive = reader.IsActive,
            PhoneNumber = reader.PhoneNumber,
            Email = reader.Email,
        };
    }
    /// <inheritdoc />
    public async Task UpdateReader(Guid id, Reader reader)
    {
        var readerEntity = await _appDbContext.Readers.FindAsync(id) ?? throw new EntityNotFoundException<Reader>(id);
        readerEntity.FullName = reader.FullName;
        readerEntity.PhoneNumber = reader.PhoneNumber;
        readerEntity.Email = reader.Email;
        readerEntity.ExpiryDate = reader.ExpiryDate;
        readerEntity.IsActive = reader.IsActive;
        _appDbContext.Readers.Update(readerEntity);
        await _appDbContext.SaveChangesAsync();
    }
    /// <inheritdoc />
    public async Task<Reader> GetReaderWithBorrowBooks(Guid id)
    {
        var readerEntity = await _appDbContext.Readers
            .Include(r => r.BorrowedRecords
                .Where(b => b.Status == BookIssueStatus.Issued))
            .ThenInclude(b => b.Book)
            .SingleOrDefaultAsync(r => r.Id == id) ?? throw new EntityNotFoundException<Reader>(id);
        var reader = new Reader
        {
            FullName = readerEntity.FullName,
            PhoneNumber = readerEntity.PhoneNumber,
            Email = readerEntity.Email,
            ExpiryDate = readerEntity.ExpiryDate,
            IsActive = readerEntity.IsActive,
            BorrowBooks = readerEntity.BorrowedRecords
                .Select(b => b.Book.ToBook())
                .ToList()
        };
        return reader;
    }
    /// <inheritdoc />
    public async Task<IReadOnlyList<BorrowedBook>> GetReadersBorrowBooks(Guid id)
    {
        var readerEntity = await _appDbContext.Readers
            .Include(r => r.BorrowedRecords)
            .ThenInclude(b => b.Book)
            .Select(r => new { r.Id, r.BorrowedRecords })
            .SingleOrDefaultAsync(r => r.Id == id) ?? throw new EntityNotFoundException<Reader>(id);
        
        return readerEntity.BorrowedRecords
            .Select(b => new BorrowedBook 
            {
                Title = b.Book.Title,
                Authors = b.Book.Authors,
                Category = b.Book.Category,
                Description = b.Book.Description,
                Year = b.Book.Year,
                DueDate = b.DueDate,
                Status = b.Status,
                BorrowDate = b.BorrowDate,
                ReturnDate = b.ReturnDate
            })
            .ToList();
    }
}
