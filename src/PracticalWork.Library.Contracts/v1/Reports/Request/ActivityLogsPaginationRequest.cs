using PracticalWork.Library.Contracts.v1.Abstracts;

namespace PracticalWork.Library.Contracts.v1.Reports.Request
{
    /// <summary>
    /// Модель запроса для получения журналов активности с поддержкой пагинации и фильтрации
    /// </summary>
    public class ActivityLogsPaginationRequest : PaginationRequest
    {
        /// <summary>
        /// Начальная дата события для фильтрации (включительно)
        /// </summary>
        public DateOnly? EventDateFrom { get; set; } 

        /// <summary>
        /// Конечная дата события для фильтрации (включительно)
        /// </summary>
        public DateOnly? EventDateTo { get; set; }

        /// <summary>
        /// Массив типов событий для фильтрации
        /// </summary>
        public string[] EventTypes { get; set; }
    }
}
