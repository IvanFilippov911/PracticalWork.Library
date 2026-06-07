using Moq;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Models.ReportModels;
using PracticalWork.Library.Options;
using PracticalWork.Library.Services;
using PracticalWork.Tests.Common;

namespace PracticalWork.Library.Tests.Services;

public sealed class WeeklyAdminReportNotificationServiceTests
{
    [Fact]
    public async Task SendAsync_WhenTemplateEmailsConfigured_UsesTrimmedTemplateEmailsAndSavesLogs()
    {
        var emailBuilder = new Mock<IWeeklyAdminReportEmailBuilder>();
        var repository = new Mock<IWeeklyAdminReportRepository>();
        var emailService = new Mock<IEmailService>();
        var logs = Array.Empty<WeeklyAdminReportNotificationLogEntry>();
        emailBuilder.Setup(x => x.BuildAsync(
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<WeeklyAdminReportStatistics>(),
                It.IsAny<string>(),
                It.IsAny<WeeklyReportTemplate>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string adminEmail, DateOnly _, DateOnly _, WeeklyAdminReportStatistics _, string _, WeeklyReportTemplate _, CancellationToken _) =>
                new EmailMessage { To = adminEmail, Subject = "Weekly report" });
        emailService.Setup(x => x.SendAsync(It.Is<EmailMessage>(m => m.To == "admin1@library.local"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult(true));
        emailService.Setup(x => x.SendAsync(It.Is<EmailMessage>(m => m.To == "admin2@library.local"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult(false, "SMTP failure"));
        repository.Setup(x => x.SaveNotificationLogs(It.IsAny<IReadOnlyCollection<WeeklyAdminReportNotificationLogEntry>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyCollection<WeeklyAdminReportNotificationLogEntry>, CancellationToken>((entries, _) => logs = entries.ToArray())
            .Returns(Task.CompletedTask);
        var service = new WeeklyAdminReportNotificationService(
            emailBuilder.Object,
            repository.Object,
            emailService.Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 9, 0, 0, DateTimeKind.Utc)));

        var result = await service.SendAsync(
            new WeeklyAdminReportProcessingRequest
            {
                TimeZoneId = "UTC",
                Template = new WeeklyReportTemplate
                {
                    AdminEmails = [" admin1@library.local ", "admin2@library.local"],
                    ReportsBucketName = "weekly-reports"
                },
                EmailSettings = new EmailSettings
                {
                    AdminEmails = ["fallback@library.local"]
                }
            },
            new WeeklyAdminReportPeriod
            {
                PeriodFrom = new DateOnly(2026, 5, 19),
                PeriodTo = new DateOnly(2026, 5, 25),
                TimeZone = TimeZoneInfo.Utc
            },
            new WeeklyAdminReportStatistics { BorrowedCount = 5 },
            "https://minio/report.csv");

        Assert.Equal(2, result.AdminEmailCount);
        Assert.Equal(1, result.SuccessCount);
        Assert.Equal(1, result.FailureCount);
        Assert.Collection(
            logs,
            log =>
            {
                Assert.Equal("admin1@library.local", log.ReceiverEmail);
                Assert.True(log.IsSuccess);
                Assert.Equal(string.Empty, log.ErrorMessage);
            },
            log =>
            {
                Assert.Equal("admin2@library.local", log.ReceiverEmail);
                Assert.False(log.IsSuccess);
                Assert.Equal("SMTP failure", log.ErrorMessage);
            });
    }

    [Fact]
    public async Task SendAsync_WhenTemplateEmailsMissing_FallsBackToEmailSettings()
    {
        var emailBuilder = new Mock<IWeeklyAdminReportEmailBuilder>();
        var repository = new Mock<IWeeklyAdminReportRepository>();
        var emailService = new Mock<IEmailService>();
        emailBuilder.Setup(x => x.BuildAsync(
                "fallback@library.local",
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<WeeklyAdminReportStatistics>(),
                It.IsAny<string>(),
                It.IsAny<WeeklyReportTemplate>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailMessage { To = "fallback@library.local", Subject = "Weekly report" });
        emailService.Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult(true));
        var service = new WeeklyAdminReportNotificationService(
            emailBuilder.Object,
            repository.Object,
            emailService.Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 9, 0, 0, DateTimeKind.Utc)));

        var result = await service.SendAsync(
            new WeeklyAdminReportProcessingRequest
            {
                TimeZoneId = "UTC",
                Template = new WeeklyReportTemplate
                {
                    AdminEmails = [" ", ""],
                    ReportsBucketName = "weekly-reports"
                },
                EmailSettings = new EmailSettings
                {
                    AdminEmails = [" fallback@library.local "]
                }
            },
            new WeeklyAdminReportPeriod
            {
                PeriodFrom = new DateOnly(2026, 5, 19),
                PeriodTo = new DateOnly(2026, 5, 25),
                TimeZone = TimeZoneInfo.Utc
            },
            new WeeklyAdminReportStatistics(),
            "https://minio/report.csv");

        Assert.Equal(1, result.AdminEmailCount);
        Assert.Equal(1, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
        emailBuilder.Verify(x => x.BuildAsync(
            "fallback@library.local",
            It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(),
            It.IsAny<WeeklyAdminReportStatistics>(),
            It.IsAny<string>(),
            It.IsAny<WeeklyReportTemplate>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WhenNoAdminEmailsConfigured_ReturnsEmptySummaryWithoutSideEffects()
    {
        var emailBuilder = new Mock<IWeeklyAdminReportEmailBuilder>(MockBehavior.Strict);
        var repository = new Mock<IWeeklyAdminReportRepository>(MockBehavior.Strict);
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);
        var service = new WeeklyAdminReportNotificationService(
            emailBuilder.Object,
            repository.Object,
            emailService.Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 9, 0, 0, DateTimeKind.Utc)));

        var result = await service.SendAsync(
            new WeeklyAdminReportProcessingRequest
            {
                TimeZoneId = "UTC",
                Template = new WeeklyReportTemplate
                {
                    AdminEmails = [" ", ""],
                    ReportsBucketName = "weekly-reports"
                },
                EmailSettings = new EmailSettings
                {
                    AdminEmails = [" ", ""]
                }
            },
            new WeeklyAdminReportPeriod
            {
                PeriodFrom = new DateOnly(2026, 5, 19),
                PeriodTo = new DateOnly(2026, 5, 25),
                TimeZone = TimeZoneInfo.Utc
            },
            new WeeklyAdminReportStatistics(),
            "https://minio/report.csv");

        Assert.Equal(0, result.AdminEmailCount);
        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
    }
}
