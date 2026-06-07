using PracticalWork.Library.Abstractions.Storage;

namespace PracticalWork.Library.Data.PostgreSql.Entities;

/// <summary>
/// Лог отправки уведомлений.
/// </summary>
public sealed class NotificationLogEntity : EntityBase
{
    /// <summary>
    /// Тип уведомления (например, return-reminder, weekly-report).
    /// </summary>
    public string NotificationType { get; set; }

    /// <summary>
    /// Идентификатор читателя (если применимо).
    /// </summary>
    public Guid? ReaderId { get; set; }

    /// <summary>
    /// Идентификатор книги (если применимо).
    /// </summary>
    public Guid? BookId { get; set; }

    /// <summary>
    /// Идентификатор записи выдачи (если применимо).
    /// </summary>
    public Guid? BorrowId { get; set; }

    /// <summary>
    /// Email получателя.
    /// </summary>
    public string ReceiverEmail { get; set; }

    /// <summary>
    /// Время попытки отправки.
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Успешность отправки.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Текст ошибки, если отправка не удалась.
    /// </summary>
    public string ErrorMessage { get; set; }
}
