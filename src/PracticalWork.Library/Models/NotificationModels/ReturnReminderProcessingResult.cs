namespace PracticalWork.Library.Models.NotificationModels;

/// <summary>
/// Результат обработки напоминаний о возврате книг.
/// </summary>
public sealed class ReturnReminderProcessingResult
{
    /// <summary>
    /// Количество найденных кандидатов.
    /// </summary>
    public int CandidateCount { get; init; }

    /// <summary>
    /// Количество успешно отправленных напоминаний.
    /// </summary>
    public int SuccessCount { get; init; }

    /// <summary>
    /// Количество неудачных отправок.
    /// </summary>
    public int FailureCount { get; init; }

    /// <summary>
    /// Количество пропущенных кандидатов.
    /// </summary>
    public int SkippedCount { get; init; }
}
