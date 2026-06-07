using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Models.ReportModels;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Services.Email;

/// <summary>
/// Сборщик email для еженедельного административного отчета.
/// </summary>
public sealed class WeeklyAdminReportEmailBuilder : IWeeklyAdminReportEmailBuilder
{
    private readonly EmailTemplateRenderer _templateRenderer;

    public WeeklyAdminReportEmailBuilder(EmailTemplateRenderer templateRenderer)
    {
        _templateRenderer = templateRenderer;
    }

    public async Task<EmailMessage> BuildAsync(
        string adminEmail,
        DateOnly periodFrom,
        DateOnly periodTo,
        WeeklyAdminReportStatistics statistics,
        string reportUrl,
        WeeklyReportTemplate template,
        CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["PeriodFrom"] = periodFrom.ToString("dd.MM.yyyy"),
            ["PeriodTo"] = periodTo.ToString("dd.MM.yyyy"),
            ["NewBooksCount"] = statistics.NewBooksCount.ToString(),
            ["NewReadersCount"] = statistics.NewReadersCount.ToString(),
            ["BorrowedCount"] = statistics.BorrowedCount.ToString(),
            ["ReturnedCount"] = statistics.ReturnedCount.ToString(),
            ["OverdueCount"] = statistics.OverdueCount.ToString(),
            ["ReportUrl"] = reportUrl
        };

        var subject = template.SubjectTemplate
            .Replace("{StartDate}", periodFrom.ToString("dd.MM.yyyy"), StringComparison.Ordinal)
            .Replace("{EndDate}", periodTo.ToString("dd.MM.yyyy"), StringComparison.Ordinal);

        var htmlBody = await _templateRenderer.RenderAsync("weekly-report.html", placeholders, cancellationToken);
        var textBody = await _templateRenderer.RenderAsync("weekly-report.txt", placeholders, cancellationToken);

        return new EmailMessage
        {
            To = adminEmail,
            Subject = subject,
            HtmlBody = htmlBody,
            TextBody = textBody
        };
    }
}
