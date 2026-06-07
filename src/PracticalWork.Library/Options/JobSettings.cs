namespace PracticalWork.Library.Options;

/// <summary>
/// Настройки фоновых задач.
/// </summary>
public class JobSettings
{
    public string TimeZoneId { get; set; } = "Europe/Moscow";

    public Dictionary<string, JobConfiguration> Jobs { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// Конфигурация отдельной фоновой задачи.
/// </summary>
public class JobConfiguration
{
    public string CronExpression { get; set; } = string.Empty;

    public int MaxRetries { get; set; } = 3;

    public int TimeoutMinutes { get; set; } = 30;
}
