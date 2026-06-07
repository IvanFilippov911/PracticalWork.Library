using PracticalWork.Library.Models.NotificationModels;

namespace PracticalWork.Library.Abstractions.Storage;

/// <summary>
/// Репозиторий данных для напоминаний о возврате книг.
/// </summary>
public interface IReturnReminderRepository
{
    /// <summary>
    /// Получить кандидатов на отправку напоминания.
    /// </summary>
    /// <param name="dueDate">Дата возврата книги.</param>
    /// <param name="duplicateCutoff">Нижняя граница окна дедупликации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<ReturnReminderCandidate>> GetCandidates(
        DateOnly dueDate,
        DateTime duplicateCutoff,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохранить логи отправки напоминаний.
    /// </summary>
    /// <param name="logs">Логи уведомлений.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task SaveNotificationLogs(
        IReadOnlyCollection<ReturnReminderNotificationLogEntry> logs,
        CancellationToken cancellationToken = default);
}
