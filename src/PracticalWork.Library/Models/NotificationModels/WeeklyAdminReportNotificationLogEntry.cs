namespace PracticalWork.Library.Models.NotificationModels;

/// <summary>
/// Лог отправки уведомления с еженедельным отчетом.
/// </summary>
public sealed class WeeklyAdminReportNotificationLogEntry
{
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
