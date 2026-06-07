namespace PracticalWork.Library.Models.Common;

/// <summary>
/// Результат постраничного запроса для application-слоя.
/// </summary>
/// <typeparam name="T">Тип сущности.</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Список сущностей текущей страницы.
    /// </summary>
    public required IReadOnlyList<T> Entities { get; init; }

    /// <summary>
    /// Общее количество записей.
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    /// Номер страницы.
    /// </summary>
    public required int PageNumber { get; init; }

    /// <summary>
    /// Размер страницы.
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// Общее количество страниц.
    /// </summary>
    public int PageCount
        => PageSize <= 0
            ? 0
            : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
