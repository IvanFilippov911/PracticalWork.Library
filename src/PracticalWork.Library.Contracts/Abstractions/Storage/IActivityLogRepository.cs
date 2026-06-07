using PracticalWork.Library.Contracts.Models.ReportModels;
using PracticalWork.Library.Contracts.v1.Reports.Request;

namespace PracticalWork.Library.Contracts.Abstractions.Storage;

/// <summary>
/// Репозиторий получения данных записей событий системы.
/// </summary>
public interface IActivityLogRepository
{
    /// <summary>
    /// Сохраняет запись о событии системы.
    /// </summary>
    /// <param name="activityLog">Сохраняемая запись.</param>
    Task AddLogAsync(ActivityLog activityLog);

    /// <summary>
    /// Возвращает страницу журналов активности по параметрам запроса.
    /// </summary>
    /// <param name="request">Параметры пагинации и фильтрации.</param>
    /// <returns>Список записей и общее количество элементов.</returns>
    Task<(IReadOnlyList<ActivityLog>, int)> GetLogsPageAsync(ActivityLogsPaginationRequest request);

    /// <summary>
    /// Возвращает записи журналов активности за период и по типам событий.
    /// </summary>
    /// <param name="periodFrom">Начало периода.</param>
    /// <param name="periodTo">Конец периода.</param>
    /// <param name="eventTypes">Набор типов событий.</param>
    /// <returns>Список записей и общее количество элементов.</returns>
    Task<(IReadOnlyList<ActivityLog>, int)> GetLogsAsync(
        DateOnly? periodFrom,
        DateOnly? periodTo,
        string[] eventTypes);
}
