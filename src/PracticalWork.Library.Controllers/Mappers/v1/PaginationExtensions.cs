using PracticalWork.Library.Models.Common;

namespace PracticalWork.Library.Controllers.Mappers.v1;

/// <summary>
/// Расширения для работы с объектами пагинации
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Преобразует объект запроса пагинации из версии v1 контракта в application-модель.
    /// </summary>
    /// <param name="request">Объект запроса пагинации</param>
    /// <returns>Модель пагинации application-слоя</returns>
    public static PageRequest ToPageRequest(this Contracts.v1.Abstracts.PaginationRequest request) =>
        new()
        {
            PageSize = request.PageSize,
            PageNumber = request.PageNumber,
        };
}
