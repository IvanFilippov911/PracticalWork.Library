using PracticalWork.Library.Models.NotificationModels;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис для отправки email сообщений.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Отправить email сообщение.
    /// </summary>
    Task<EmailSendResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
