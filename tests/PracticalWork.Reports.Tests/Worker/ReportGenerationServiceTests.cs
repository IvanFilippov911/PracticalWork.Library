using Moq;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.Abstractions.Storage;
using PracticalWork.Library.Contracts.Models.ReportModels;
using PracticalWork.Library.Contracts.Options;
using PracticalWork.Library.Contracts.v1.Enums;
using PracticalWork.Reports.Worker.Abstractions;
using PracticalWork.Reports.Worker.Models;
using PracticalWork.Reports.Worker.Services;
using PracticalWork.Tests.Common;

namespace PracticalWork.Reports.Tests.Worker;

public sealed class ReportGenerationServiceTests
{
    [Fact]
    public async Task GenerateReportAsync_WhenSuccessful_UploadsFileUpdatesReportAndInvalidatesCache()
    {
        var activityLogRepository = new Mock<IActivityLogRepository>();
        var reportRepository = new Mock<IReportRepository>();
        var minioService = new Mock<IMinioService>();
        var generateService = new Mock<IReportGenerateService>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        var report = new Report
        {
            Status = ReportStatus.InProgress,
            EventTypes = ["book.created"]
        };
        reportRepository.Setup(x => x.GetReportById(It.IsAny<Guid>())).ReturnsAsync(report);
        activityLogRepository.Setup(x => x.GetLogsAsync(It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<string[]>()))
            .ReturnsAsync((new List<ActivityLog>
            {
                new()
                {
                    EventType = "book.created",
                    EventDate = new DateTime(2026, 5, 20, 8, 0, 0, DateTimeKind.Utc),
                    Event = null!
                }
            }, 1));
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        generateService.Setup(x => x.GenerateReport(It.IsAny<Guid>(), It.IsAny<IReadOnlyList<ActivityLog>>()))
            .Returns(new ReportGenerateResult
            {
                FileName = "2026/5/report-id.csv",
                Content = stream,
                ContentType = "text/csv"
            });

        var service = CreateService(activityLogRepository, reportRepository, minioService, generateService, cacheVersionService);
        var reportId = Guid.NewGuid();

        await service.GenerateReportAsync(reportId, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 7), ["book.created"], CancellationToken.None);

        minioService.Verify(x => x.UploadFileAsync("reports-bucket", "2026/5/report-id.csv", stream, "text/csv", It.IsAny<CancellationToken>()), Times.Once);
        reportRepository.Verify(x => x.UpdateReport(reportId, It.Is<Report>(r => r.Status == ReportStatus.Generated && r.Name == "report-id.csv")), Times.Once);
        cacheVersionService.Verify(x => x.IncrementVersionAsync("reports", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateReportAsync_WhenGeneratorFails_MarksReportAsErrorAndInvalidatesCache()
    {
        var activityLogRepository = new Mock<IActivityLogRepository>();
        var reportRepository = new Mock<IReportRepository>();
        var minioService = new Mock<IMinioService>();
        var generateService = new Mock<IReportGenerateService>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        var report = new Report { Status = ReportStatus.InProgress };
        reportRepository.Setup(x => x.GetReportById(It.IsAny<Guid>())).ReturnsAsync(report);
        activityLogRepository.Setup(x => x.GetLogsAsync(It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<string[]>()))
            .ReturnsAsync((new List<ActivityLog>(), 0));
        generateService.Setup(x => x.GenerateReport(It.IsAny<Guid>(), It.IsAny<IReadOnlyList<ActivityLog>>()))
            .Throws(new InvalidOperationException("generation failed"));

        var service = CreateService(activityLogRepository, reportRepository, minioService, generateService, cacheVersionService);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GenerateReportAsync(Guid.NewGuid(), null, null, ["book.created"], CancellationToken.None));

        reportRepository.Verify(x => x.UpdateReport(It.IsAny<Guid>(), It.Is<Report>(r => r.Status == ReportStatus.Error)), Times.Once);
        cacheVersionService.Verify(x => x.IncrementVersionAsync("reports", It.IsAny<CancellationToken>()), Times.Once);
    }

    private static ReportGenerationService CreateService(
        Mock<IActivityLogRepository> activityLogRepository,
        Mock<IReportRepository> reportRepository,
        Mock<IMinioService> minioService,
        Mock<IReportGenerateService> generateService,
        Mock<ICacheVersionService> cacheVersionService)
    {
        return new ReportGenerationService(
            activityLogRepository.Object,
            reportRepository.Object,
            minioService.Object,
            generateService.Object,
            cacheVersionService.Object,
            new StaticOptionsMonitor<MinioOptions>(new MinioOptions { ReportsBucketName = "reports-bucket" }),
            new StaticOptionsMonitor<BooksCacheOptions>(new BooksCacheOptions
            {
                ReportsCacheOptions = new CacheOptions { Prefix = "reports", TtlMinutes = 15 }
            }),
            new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));
    }
}
