namespace PracticalWork.Library.Models.JobModels;

/// <summary>
/// Информация о настроенной фоновой задаче.
/// </summary>
public sealed class ScheduledJobInfo
{
    /// <summary>Имя задачи.</summary>
    public required string Name { get; init; }

    /// <summary>Описание задачи.</summary>
    public required string Description { get; init; }

    /// <summary>Cron-выражение задачи.</summary>
    public required string CronExpression { get; init; }

    /// <summary>Таймзона задачи.</summary>
    public required string TimeZoneId { get; init; }
}
