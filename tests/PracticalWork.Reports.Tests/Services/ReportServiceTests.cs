using Moq;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.Abstractions.Storage;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.Contracts.Models.ReportModels;
using PracticalWork.Library.Contracts.Options;
using PracticalWork.Library.Contracts.v1.Enums;
using PracticalWork.Library.Contracts.v1.Reports.Request;
using PracticalWork.Library.Contracts.v1.Reports.Response;
using PracticalWork.Reports.Web.Services;
using PracticalWork.Tests.Common;

namespace PracticalWork.Reports.Tests.Services;

public sealed class ReportServiceTests
{
    [Fact]
    public async Task ReadSystemActivityLogs_WhenLogsReturned_MapsEntitiesAndPagination()
    {
        var activityLogRepository = new Mock<IActivityLogRepository>();
        var request = new ActivityLogsPaginationRequest
        {
            PageNumber = 2,
            PageSize = 50,
            EventTypes = ["book.created"]
        };
        var activityLog = new ActivityLog
        {
            EventType = "book.created",
            EventDate = new DateTime(2026, 5, 21, 9, 30, 0, DateTimeKind.Utc),
            Event = new BookCreatedEvent(Guid.NewGuid(), "Book", "ScientificBook", ["Author"], 2026)
        };
        activityLogRepository.Setup(x => x.GetLogsPageAsync(request))
            .ReturnsAsync((new List<ActivityLog> { activityLog }, 1));
        var service = CreateService(
            activityLogRepository,
            new Mock<IReportRepository>(),
            new Mock<IMessageProducer>(),
            new Mock<IMinioService>(),
            new Mock<ICacheService>(),
            new Mock<ICacheVersionService>());

        var result = await service.ReadSystemActivityLogs(request);

        Assert.Single(result.Entities);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(50, result.PageSize);
        Assert.Equal("book.created", result.Entities[0].EventType);
        Assert.Equal(activityLog.EventDate, result.Entities[0].EventDate);
    }

    [Fact]
    public async Task GetListOfReadyReports_WhenCacheHasData_ReturnsCacheWithoutRepositoryCall()
    {
        var activityLogRepository = new Mock<IActivityLogRepository>(MockBehavior.Strict);
        var reportRepository = new Mock<IReportRepository>(MockBehavior.Strict);
        var producer = new Mock<IMessageProducer>(MockBehavior.Strict);
        var minioService = new Mock<IMinioService>(MockBehavior.Strict);
        var cacheService = new Mock<ICacheService>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        cacheVersionService.Setup(x => x.GetVersionAsync("reports", It.IsAny<CancellationToken>())).ReturnsAsync(2);
        cacheService
            .Setup(x => x.GetAsync<List<ReportResponse>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new ReportResponse("cached.csv", "https://cached", new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 7), ["book.created"], DateTime.UtcNow)
            ]);

        var service = CreateService(activityLogRepository, reportRepository, producer, minioService, cacheService, cacheVersionService);

        var result = await service.GetListOfReadyReports();

        Assert.Single(result);
        Assert.Equal("cached.csv", result[0].ReportName);
        reportRepository.Verify(x => x.GetReadyReports(), Times.Never);
    }

    [Fact]
    public async Task GetListOfReadyReports_WhenCacheEmpty_LoadsRepositoryAndWritesCache()
    {
        var activityLogRepository = new Mock<IActivityLogRepository>(MockBehavior.Strict);
        var reportRepository = new Mock<IReportRepository>();
        var producer = new Mock<IMessageProducer>(MockBehavior.Strict);
        var minioService = new Mock<IMinioService>(MockBehavior.Strict);
        var cacheService = new Mock<ICacheService>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        cacheVersionService.Setup(x => x.GetVersionAsync("reports", It.IsAny<CancellationToken>())).ReturnsAsync(3);
        cacheService
            .Setup(x => x.GetAsync<List<ReportResponse>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        reportRepository.Setup(x => x.GetReadyReports()).ReturnsAsync([
            new Report
            {
                Name = "generated.csv",
                FilePath = "https://minio/generated.csv",
                PeriodFrom = new DateOnly(2026, 5, 1),
                PeriodTo = new DateOnly(2026, 5, 7),
                EventTypes = ["book.created"],
                GeneratedAt = new DateTime(2026, 5, 22, 12, 0, 0, DateTimeKind.Utc)
            }
        ]);
        var service = CreateService(activityLogRepository, reportRepository, producer, minioService, cacheService, cacheVersionService);

        var result = await service.GetListOfReadyReports();

        Assert.Single(result);
        Assert.Equal("generated.csv", result[0].ReportName);
        cacheService.Verify(x => x.SetAsync(
            It.IsAny<string>(),
            It.Is<List<ReportResponse>>(reports => reports.Count == 1 && reports[0].ReportName == "generated.csv"),
            It.IsAny<TimeSpan>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateReport_WhenSuccessful_PersistsPublishesEventAndInvalidatesCache()
    {
        var reportRepository = new Mock<IReportRepository>();
        var producer = new Mock<IMessageProducer>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        var service = CreateService(
            new Mock<IActivityLogRepository>(),
            reportRepository,
            producer,
            new Mock<IMinioService>(),
            new Mock<ICacheService>(),
            cacheVersionService);
        var reportId = Guid.NewGuid();
        reportRepository.Setup(x => x.CreateReport(It.IsAny<Report>())).ReturnsAsync(reportId);

        var response = await service.CreateReport(new ReportCreateRequest(
            new DateOnly(2026, 5, 1),
            new DateOnly(2026, 5, 7),
            ["book.created", "reader.created"]));

        Assert.Equal(ReportStatus.InProgress, response.Status);
        Assert.Equal(2, response.EventTypes.Count);
        producer.Verify(x => x.ProduceReportCreateAsync(It.Is<ReportCreateEvent>(e => e.Id == reportId)), Times.Once);
        cacheVersionService.Verify(x => x.IncrementVersionAsync("reports", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetReportUrl_WhenSuccessful_UpdatesRepositoryAndInvalidatesCache()
    {
        var reportRepository = new Mock<IReportRepository>();
        var minioService = new Mock<IMinioService>();
        var cacheVersionService = new Mock<ICacheVersionService>();
        var report = new Report
        {
            Name = "report.csv",
            GeneratedAt = new DateTime(2026, 5, 24, 12, 0, 0, DateTimeKind.Utc)
        };
        reportRepository.Setup(x => x.GetReportByName("report.csv")).ReturnsAsync((Guid.NewGuid(), report));
        minioService.Setup(x => x.GetFileUrlAsync("reports-bucket", "2026/5/report.csv", It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://minio/report.csv");
        var service = CreateService(
            new Mock<IActivityLogRepository>(),
            reportRepository,
            new Mock<IMessageProducer>(),
            minioService,
            new Mock<ICacheService>(),
            cacheVersionService);

        var url = await service.GetReportUrl("report.csv");

        Assert.Equal("https://minio/report.csv", url);
        Assert.Equal("https://minio/report.csv", report.FilePath);
        reportRepository.Verify(x => x.UpdateReport(It.IsAny<Guid>(), report), Times.Once);
        cacheVersionService.Verify(x => x.IncrementVersionAsync("reports", It.IsAny<CancellationToken>()), Times.Once);
    }

    private static ReportService CreateService(
        Mock<IActivityLogRepository> activityLogRepository,
        Mock<IReportRepository> reportRepository,
        Mock<IMessageProducer> producer,
        Mock<IMinioService> minioService,
        Mock<ICacheService> cacheService,
        Mock<ICacheVersionService> cacheVersionService)
    {
        return new ReportService(
            activityLogRepository.Object,
            reportRepository.Object,
            producer.Object,
            cacheService.Object,
            minioService.Object,
            new StaticOptionsMonitor<MinioOptions>(new MinioOptions { ReportsBucketName = "reports-bucket" }),
            cacheVersionService.Object,
            new StaticOptionsMonitor<BooksCacheOptions>(new BooksCacheOptions
            {
                ReportsCacheOptions = new CacheOptions { Prefix = "reports", TtlMinutes = 15 }
            }),
            new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));
    }
}
