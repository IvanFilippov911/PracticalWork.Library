using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сборщик email для напоминания о возврате книги.
/// </summary>
public interface IReturnReminderEmailBuilder
{
    /// <summary>
    /// Собрать email-сообщение для кандидата.
    /// </summary>
    /// <param name="candidate">Кандидат на отправку.</param>
    /// <param name="template">Настройки шаблона.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<EmailMessage> BuildAsync(
        ReturnReminderCandidate candidate,
        ReturnReminderTemplate template,
        CancellationToken cancellationToken = default);
}
