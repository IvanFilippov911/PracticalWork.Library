namespace PracticalWork.Library.Contracts.v1.Abstracts
{
    /// <summary>
    /// Базовый ответ с пагинацией
    /// </summary>
    /// <typeparam name="T">Тип сущностей</typeparam>
    public class PaginationResponse<T>
    {
        /// <summary>
        /// Список сущностей
        /// </summary>
        public List<T> Entities { get; set; }

        /// <summary>
        /// Общее количество записей
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Количество страниц
        /// </summary>
        public int PageCount { get; set; }
        
        /// <summary>
        /// Номер страницы
        /// </summary>
        public int PageNumber { get; set; }
        
        /// <summary>
        /// Размер страницы
        /// </summary>
        public int PageSize { get; set; }
    }
}