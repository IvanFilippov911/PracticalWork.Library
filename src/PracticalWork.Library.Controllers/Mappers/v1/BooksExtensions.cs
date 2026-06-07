using Microsoft.AspNetCore.Http;
using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Contracts.v1.Books.Response;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.Common;
using BookIssueStatus = PracticalWork.Library.Contracts.v1.Enums.BookIssueStatus;

namespace PracticalWork.Library.Controllers.Mappers.v1
{
    /// <summary>
    /// Расширения для преобразования объектов, связанных с книгами
    /// </summary>
    public static class BooksExtensions
    {
        /// <summary>
        /// Преобразует запрос на создание книги в модель Book
        /// </summary>
        /// <param name="request">Объект запроса CreateBookRequest</param>
        /// <returns>Экземпляр модели Book</returns>
        public static Book ToBook(this CreateBookRequest request) =>
            new()
            {
                Authors = request.Authors,
                Title = request.Title,
                Description = request.Description,
                Year = request.Year,
                Category = (BookCategory)request.Category
            };

        
        /// <summary>
        /// Преобразует запрос на обновление книги в модель Book
        /// </summary>
        /// <param name="request">Объект запроса UpdateBookRequest</param>
        /// <returns>Экземпляр модели Book</returns>
        public static Book ToBook(this UpdateBookRequest request) =>
            new()
            {
                Authors = request.Authors,
                Title = request.Title,
                Description = request.Description,
                Year = request.Year,
            };

        /// <summary>
        /// Преобразует запрос на получение книг в application-модель критериев поиска.
        /// </summary>
        /// <param name="request">Объект запроса GetBooksRequest</param>
        /// <returns>Критерии поиска книг</returns>
        public static BookSearchCriteria ToBookSearchCriteria(this GetBooksRequest request)
            => new()
            {
                Author = request.Author,
                Category = request.Category == null ? null: (BookCategory)request.Category,
                Status = request.Status == null ? null : (BookStatus)request.Status,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
            };

        /// <summary>
        /// Преобразует строковый идентификатор или название книги в критерий поиска деталей.
        /// </summary>
        public static BookDetailsQuery ToBookDetailsQuery(this string idOrTitle)
            => Guid.TryParse(idOrTitle, out var bookId)
                ? new BookDetailsQuery { BookId = bookId }
                : new BookDetailsQuery { Title = idOrTitle };

        /// <summary>
        /// Преобразует IFormFile в application-модель загруженной обложки.
        /// </summary>
        public static BookCoverUpload ToBookCoverUpload(this IFormFile file)
            => file is null
                ? BookCoverUpload.Empty
                : new BookCoverUpload(
                    file.FileName,
                    file.ContentType,
                    file.OpenReadStream());

        /// <summary>
        /// Преобразует результат страницы книг в контракт v1 с пагинацией.
        /// </summary>
        public static Contracts.v1.Abstracts.PaginationResponse<BookResponse> ToBookPaginationResponse(
            this PagedResult<Book> booksPaginationResponse)
            => new()
            {
                Entities = booksPaginationResponse.Entities
                    .Select(ToBookResponse)
                    .ToList(),
                
                PageCount = booksPaginationResponse.PageCount,
                TotalCount = booksPaginationResponse.TotalCount,
                PageSize = booksPaginationResponse.PageSize,
                PageNumber = booksPaginationResponse.PageNumber,
            };
        
        /// <summary>
        /// Преобразует модель Book в DTO ответа BookResponse
        /// </summary>
        /// <param name="book">Модель книги</param>
        /// <returns>DTO ответа BookResponse</returns>
        public static BookResponse ToBookResponse(this Book book) =>
            new BookResponse(
                book.Title, 
                (Contracts.v1.Enums.BookCategory)book.Category, 
                book.Authors, book.Description, 
                book.Year, 
                (Contracts.v1.Enums.BookStatus)book.Status, 
                book.IsArchived);
    
        /// <summary>
        /// Преобразует модель BookArchive в DTO ответа ArchiveBookResponse
        /// </summary>
        public static ArchiveBookResponse ToArchiveBookResponse(this BookArchive book) =>
            new(book.Id, book.Title, book.ArchivedAt);
    
        /// <summary>
        /// Преобразует модель Book в DTO ответа с деталями книги
        /// </summary>
        public static BookDetailsResponse ToBookDetailsResponse(this Book book, Guid id) =>
            new (
                id,
                book.Title,
                (Contracts.v1.Enums.BookCategory)book.Category,
                book.Authors,
                book.Description,
                book.Year,
                book.CoverImagePath,
                (Contracts.v1.Enums.BookStatus)book.Status,
                book.IsArchived
                );

        /// <summary>
        /// Преобразует результат страницы книг с историей выдач в контракт v1 с пагинацией.
        /// </summary>
        public static Contracts.v1.Abstracts.PaginationResponse<BookWithIssuanceRecordsResponse>
            ToBookWithIssuanceCursorPaginationResponse(this PagedResult<Book> response) =>
                new()
                {
                    Entities = response.Entities.Select(ToBookWithIssuanceRecordsResponse).ToList(),
                    PageCount = response.PageCount,
                    TotalCount = response.TotalCount,
                    PageSize = response.PageSize,
                    PageNumber = response.PageNumber,
                };
        
        /// <summary>
        /// Преобразует модель Book в DTO ответа с историей выдач книги
        /// </summary>
        public static BookWithIssuanceRecordsResponse ToBookWithIssuanceRecordsResponse(this Book book) => 
            new BookWithIssuanceRecordsResponse(
                book.Title, 
                (Contracts.v1.Enums.BookCategory)book.Category, 
                book.Authors, book.Description, 
                book.Year, 
                (Contracts.v1.Enums.BookStatus)book.Status, 
                book.IsArchived,
                book.IssuanceRecords
                    .Select(i => i.ToIssuanceRecord())
                    .ToList()
                );
    
        /// <summary>
        /// Преобразует модель BookBorrow в DTO записи о выдаче книги
        /// </summary>
        public static IssuanceRecord ToIssuanceRecord(this BookBorrow book) =>
            new((BookIssueStatus)book.Status, book.DueDate, book.ReturnDate, book.BorrowDate);
        
    
        /// <summary>
        /// Преобразует модель BorrowedBook в DTO ответа BorrowedBookResponse
        /// </summary>
        public static BorrowedBookResponse ToBorrowedBookResponse(this BorrowedBook book) =>
            new (
                book.Title, 
                (Contracts.v1.Enums.BookCategory)book.Category, 
                book.Authors,
                book.Description, 
                book.Year, 
                (BookIssueStatus)book.Status, 
                book.DueDate, 
                book.ReturnDate, 
                book.BorrowDate);
    }
}
