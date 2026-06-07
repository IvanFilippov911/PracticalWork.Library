using PracticalWork.Library.Options;

namespace PracticalWork.Library.Models.ReportModels;

/// <summary>
/// Запрос на подготовку еженедельного административного отчета.
/// </summary>
public sealed class WeeklyAdminReportProcessingRequest
{
    /// <summary>
    /// Идентификатор временной зоны.
    /// </summary>
    public required string TimeZoneId { get; init; }

    /// <summary>
    /// Настройки weekly report шаблона.
    /// </summary>
    public required WeeklyReportTemplate Template { get; init; }

    /// <summary>
    /// Настройки email.
    /// </summary>
    public required EmailSettings EmailSettings { get; init; }
}
