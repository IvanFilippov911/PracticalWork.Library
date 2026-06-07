using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Books.Request
{
    /// <summary>
    /// Модель запроса для получения списка книг с поддержкой фильтрации и пагинации
    /// </summary>
    public class GetBooksRequest : PaginationRequest
    {
        /// <summary>
        /// Статус
        /// </summary>xxx
        public BookStatus? Status { get; set; }
        
        /// <summary>
        /// Категория
        /// </summary>
        public BookCategory? Category { get; set; }
        
        /// <summary>
        /// Автор
        /// </summary>
        public string Author { get; set; }
    }
}