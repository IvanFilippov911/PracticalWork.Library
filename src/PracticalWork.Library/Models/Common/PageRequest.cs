using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Models.Common;

/// <summary>
/// Базовая модель пагинации для application-слоя.
/// </summary>
public class PageRequest
{
    private int _pageNumber = PaginationDefaults.PageNumber;
    private int _pageSize = PaginationDefaults.PageSize;

    /// <summary>
    /// Номер страницы, начиная с 1.
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value > 0 ? value : PaginationDefaults.PageNumber;
    }

    /// <summary>
    /// Размер страницы.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 0 ? value : PaginationDefaults.PageSize;
    }
}
