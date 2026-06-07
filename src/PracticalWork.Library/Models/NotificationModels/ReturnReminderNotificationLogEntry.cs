namespace PracticalWork.Library.Models.NotificationModels;

/// <summary>
/// Лог отправки напоминания о возврате книги.
/// </summary>
public sealed class ReturnReminderNotificationLogEntry
{
    /// <summary>
    /// Идентификатор читателя.
    /// </summary>
    public Guid ReaderId { get; init; }

    /// <summary>
    /// Идентификатор книги.
    /// </summary>
    public Guid BookId { get; init; }

    /// <summary>
    /// Идентификатор выдачи.
    /// </summary>
    public Guid BorrowId { get; init; }

    /// <summary>
    /// Email получателя.
    /// </summary>
    public required string ReceiverEmail { get; init; }

    /// <summary>
    /// Время попытки отправки.
    /// </summary>
    public DateTime SentAt { get; init; }

    /// <summary>
    /// Признак успешной отправки.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Текст ошибки.
    /// </summary>
    public string ErrorMessage { get; init; }
}
