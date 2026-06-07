using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис сценария отправки напоминаний о возврате книг.
/// </summary>
public interface IReturnReminderProcessingService
{
    /// <summary>
    /// Выполнить обработку и отправку напоминаний.
    /// </summary>
    /// <param name="dueDate">Дата возврата книг для напоминаний.</param>
    /// <param name="duplicateCutoff">Нижняя граница дедупликации.</param>
    /// <param name="template">Настройки шаблона.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<ReturnReminderProcessingResult> ProcessAsync(
        DateOnly dueDate,
        DateTime duplicateCutoff,
        ReturnReminderTemplate template,
        CancellationToken cancellationToken = default);
}
