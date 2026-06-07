using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Abstracts
{
    /// <summary>
    /// Модель запроса для пагинации данных
    /// </summary>
    public class PaginationRequest
    {
        private int _pageNumber;
        private int _pageSize;
        
               
        /// <summary>
        /// Номер страницы, начиная с 1
        /// </summary>
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > 0
                ? value
                : PaginationDefaults.PageNumber;
        }

        /// <summary>
        /// Размер страницы
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 0
                ? value
                : PaginationDefaults.PageSize;
        }
    }
}