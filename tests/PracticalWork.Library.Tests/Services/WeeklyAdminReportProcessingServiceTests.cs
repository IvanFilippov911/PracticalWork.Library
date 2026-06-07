using Moq;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models.ReportModels;
using PracticalWork.Library.Services;

namespace PracticalWork.Library.Tests.Services;

public sealed class WeeklyAdminReportProcessingServiceTests
{
    [Fact]
    public async Task ProcessAsync_WhenSuccessful_ComposesNestedServices()
    {
        var repository = new Mock<IWeeklyAdminReportRepository>();
        var periodService = new Mock<IWeeklyAdminReportPeriodService>();
        var storageService = new Mock<IWeeklyAdminReportStorageService>();
        var notificationService = new Mock<IWeeklyAdminReportNotificationService>();
        var period = new WeeklyAdminReportPeriod
        {
            PeriodFrom = new DateOnly(2026, 5, 18),
            PeriodTo = new DateOnly(2026, 5, 24),
            TimeZone = TimeZoneInfo.Utc
        };
        var statistics = new WeeklyAdminReportStatistics { NewBooksCount = 1, ReturnedCount = 2 };
        var artifact = new WeeklyAdminReportArtifact
        {
            ReportName = "report_2026-05-24.csv",
            ReportUrl = "https://minio/report.csv",
            BucketName = "weekly-reports",
            ObjectName = "report_2026-05-24.csv"
        };
        var summary = new WeeklyAdminReportNotificationSummary
        {
            AdminEmailCount = 2,
            SuccessCount = 1,
            FailureCount = 1
        };

        periodService.Setup(x => x.GetPreviousWeek("UTC")).Returns(period);
        repository.Setup(x => x.GetStatistics(period.PeriodFrom, period.PeriodTo, period.TimeZone, It.IsAny<CancellationToken>()))
            .ReturnsAsync(statistics);
        storageService.Setup(x => x.PublishAsync(It.IsAny<WeeklyAdminReportProcessingRequest>(), period, statistics, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artifact);
        notificationService.Setup(x => x.SendAsync(It.IsAny<WeeklyAdminReportProcessingRequest>(), period, statistics, artifact.ReportUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(summary);

        var service = new WeeklyAdminReportProcessingService(
            repository.Object,
            periodService.Object,
            storageService.Object,
            notificationService.Object);

        var result = await service.ProcessAsync(new WeeklyAdminReportProcessingRequest
        {
            TimeZoneId = "UTC",
            Template = new PracticalWork.Library.Options.WeeklyReportTemplate
            {
                ReportsBucketName = "weekly-reports",
                ReportRetentionDays = 30
            },
            EmailSettings = new PracticalWork.Library.Options.EmailSettings()
        });

        Assert.Equal("report_2026-05-24.csv", result.ReportName);
        Assert.Equal(2, result.AdminEmailCount);
        Assert.Equal(1, result.SuccessCount);
        Assert.Equal(1, result.FailureCount);
        storageService.Verify(x => x.CleanupAsync("weekly-reports", 30, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_WhenNotificationSummaryIsEmpty_ReturnsZeroCounts()
    {
        var repository = new Mock<IWeeklyAdminReportRepository>();
        var periodService = new Mock<IWeeklyAdminReportPeriodService>();
        var storageService = new Mock<IWeeklyAdminReportStorageService>();
        var notificationService = new Mock<IWeeklyAdminReportNotificationService>();
        var period = new WeeklyAdminReportPeriod
        {
            PeriodFrom = new DateOnly(2026, 5, 18),
            PeriodTo = new DateOnly(2026, 5, 24),
            TimeZone = TimeZoneInfo.Utc
        };
        var statistics = new WeeklyAdminReportStatistics();
        var artifact = new WeeklyAdminReportArtifact
        {
            ReportName = "report_2026-05-24.csv",
            ReportUrl = "https://minio/report.csv",
            BucketName = "weekly-reports",
            ObjectName = "report_2026-05-24.csv"
        };

        periodService.Setup(x => x.GetPreviousWeek(It.IsAny<string>())).Returns(period);
        repository.Setup(x => x.GetStatistics(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<TimeZoneInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statistics);
        storageService.Setup(x => x.PublishAsync(It.IsAny<WeeklyAdminReportProcessingRequest>(), It.IsAny<WeeklyAdminReportPeriod>(), It.IsAny<WeeklyAdminReportStatistics>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(artifact);
        notificationService.Setup(x => x.SendAsync(It.IsAny<WeeklyAdminReportProcessingRequest>(), It.IsAny<WeeklyAdminReportPeriod>(), It.IsAny<WeeklyAdminReportStatistics>(), artifact.ReportUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeeklyAdminReportNotificationSummary());

        var service = new WeeklyAdminReportProcessingService(
            repository.Object,
            periodService.Object,
            storageService.Object,
            notificationService.Object);

        var result = await service.ProcessAsync(new WeeklyAdminReportProcessingRequest
        {
            TimeZoneId = "UTC",
            Template = new PracticalWork.Library.Options.WeeklyReportTemplate
            {
                ReportsBucketName = "weekly-reports",
                ReportRetentionDays = 30
            },
            EmailSettings = new PracticalWork.Library.Options.EmailSettings()
        });

        Assert.Equal("report_2026-05-24.csv", result.ReportName);
        Assert.Equal(0, result.AdminEmailCount);
        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
    }
}
