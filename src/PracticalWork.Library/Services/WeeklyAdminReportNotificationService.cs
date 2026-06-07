using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Services;

/// <summary>
/// Рассылка weekly report администраторам.
/// </summary>
public sealed class WeeklyAdminReportNotificationService : IWeeklyAdminReportNotificationService
{
    private readonly IWeeklyAdminReportEmailBuilder _weeklyAdminReportEmailBuilder;
    private readonly IWeeklyAdminReportRepository _weeklyAdminReportRepository;
    private readonly IEmailService _emailService;
    private readonly TimeProvider _timeProvider;

    public WeeklyAdminReportNotificationService(
        IWeeklyAdminReportEmailBuilder weeklyAdminReportEmailBuilder,
        IWeeklyAdminReportRepository weeklyAdminReportRepository,
        IEmailService emailService,
        TimeProvider timeProvider)
    {
        _weeklyAdminReportEmailBuilder = weeklyAdminReportEmailBuilder;
        _weeklyAdminReportRepository = weeklyAdminReportRepository;
        _emailService = emailService;
        _timeProvider = timeProvider;
    }

    public async Task<WeeklyAdminReportNotificationSummary> SendAsync(
        WeeklyAdminReportProcessingRequest request,
        WeeklyAdminReportPeriod period,
        WeeklyAdminReportStatistics statistics,
        string reportUrl,
        CancellationToken cancellationToken = default)
    {
        var adminEmails = ResolveAdminEmails(request);
        if (adminEmails.Count == 0)
        {
            return new WeeklyAdminReportNotificationSummary();
        }

        var successCount = 0;
        var failureCount = 0;
        var logs = new List<WeeklyAdminReportNotificationLogEntry>();

        foreach (var adminEmail in adminEmails)
        {
            var message = await _weeklyAdminReportEmailBuilder.BuildAsync(
                adminEmail,
                period.PeriodFrom,
                period.PeriodTo,
                statistics,
                reportUrl,
                request.Template,
                cancellationToken);

            var sendResult = await _emailService.SendAsync(message, cancellationToken);
            if (sendResult.Success)
            {
                successCount++;
            }
            else
            {
                failureCount++;
            }

            logs.Add(new WeeklyAdminReportNotificationLogEntry
            {
                ReceiverEmail = adminEmail,
                SentAt = _timeProvider.GetUtcNow().UtcDateTime,
                IsSuccess = sendResult.Success,
                ErrorMessage = sendResult.ErrorMessage
            });
        }

        if (logs.Count > 0)
        {
            await _weeklyAdminReportRepository.SaveNotificationLogs(logs, cancellationToken);
        }

        return new WeeklyAdminReportNotificationSummary
        {
            AdminEmailCount = adminEmails.Count,
            SuccessCount = successCount,
            FailureCount = failureCount
        };
    }

    private static List<string> ResolveAdminEmails(WeeklyAdminReportProcessingRequest request)
    {
        var fromTemplate = request.Template.AdminEmails
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .Select(email => email.Trim())
            .ToList();

        if (fromTemplate.Count > 0)
        {
            return fromTemplate;
        }

        return request.EmailSettings.AdminEmails
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .Select(email => email.Trim())
            .ToList();
    }
}
