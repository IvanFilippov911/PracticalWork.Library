using System.Text;
using Moq;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Models.ReportModels;
using PracticalWork.Library.Options;
using PracticalWork.Library.Services;
using PracticalWork.Tests.Common;

namespace PracticalWork.Library.Tests.Services;

public sealed class WeeklyAdminReportStorageServiceTests
{
    [Fact]
    public async Task PublishAsync_WhenSuccessful_UploadsCsvStoresMetadataAndReturnsArtifact()
    {
        var repository = new Mock<IWeeklyAdminReportRepository>();
        var csvBuilder = new Mock<IWeeklyAdminReportCsvBuilder>();
        var minioService = new Mock<IMinioService>();
        string? uploadedContent = null;
        WeeklyAdminReportMetadata? savedMetadata = null;
        csvBuilder.Setup(x => x.Build(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<WeeklyAdminReportStatistics>()))
            .Returns("header1,header2\n1,2");
        minioService.Setup(x => x.UploadFileAsync(
                "weekly-reports",
                "report_2026-05-24.csv",
                It.IsAny<Stream>(),
                "text/csv",
                It.IsAny<CancellationToken>()))
            .Callback<string, string, Stream, string, CancellationToken>((_, _, stream, _, _) =>
            {
                using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
                uploadedContent = reader.ReadToEnd();
                stream.Position = 0;
            })
            .Returns(Task.CompletedTask);
        minioService.Setup(x => x.GetFileUrlAsync("weekly-reports", "report_2026-05-24.csv", It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://minio/weekly/report_2026-05-24.csv");
        repository.Setup(x => x.UpsertMetadata(It.IsAny<WeeklyAdminReportMetadata>(), It.IsAny<CancellationToken>()))
            .Callback<WeeklyAdminReportMetadata, CancellationToken>((metadata, _) => savedMetadata = metadata)
            .Returns(Task.CompletedTask);
        var service = new WeeklyAdminReportStorageService(
            repository.Object,
            csvBuilder.Object,
            minioService.Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 9, 0, 0, DateTimeKind.Utc)));

        var result = await service.PublishAsync(
            CreateRequest(),
            new WeeklyAdminReportPeriod
            {
                PeriodFrom = new DateOnly(2026, 5, 18),
                PeriodTo = new DateOnly(2026, 5, 24),
                TimeZone = TimeZoneInfo.Utc
            },
            new WeeklyAdminReportStatistics
            {
                NewBooksCount = 3,
                ReturnedCount = 7
            });

        Assert.Equal("report_2026-05-24.csv", result.ReportName);
        Assert.Equal("https://minio/weekly/report_2026-05-24.csv", result.ReportUrl);
        Assert.Equal("header1,header2\n1,2", uploadedContent);
        Assert.NotNull(savedMetadata);
        Assert.Equal(new DateOnly(2026, 5, 18), savedMetadata!.PeriodFrom);
        Assert.Equal(new DateOnly(2026, 5, 24), savedMetadata.PeriodTo);
        Assert.Equal(3, savedMetadata.Statistics.NewBooksCount);
    }

    [Fact]
    public async Task CleanupAsync_WhenNoReportsForCleanup_DoesNothing()
    {
        var repository = new Mock<IWeeklyAdminReportRepository>();
        var csvBuilder = new Mock<IWeeklyAdminReportCsvBuilder>(MockBehavior.Strict);
        var minioService = new Mock<IMinioService>(MockBehavior.Strict);
        repository.Setup(x => x.GetReportsForCleanup(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<WeeklyAdminReportCleanupCandidate>());
        var service = new WeeklyAdminReportStorageService(
            repository.Object,
            csvBuilder.Object,
            minioService.Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 9, 0, 0, DateTimeKind.Utc)));

        await service.CleanupAsync("weekly-reports", 30);

        repository.Verify(x => x.MarkReportsDeleted(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CleanupAsync_WhenSomeDeletesFail_MarksOnlySuccessfullyRemovedReports()
    {
        var repository = new Mock<IWeeklyAdminReportRepository>();
        var csvBuilder = new Mock<IWeeklyAdminReportCsvBuilder>(MockBehavior.Strict);
        var minioService = new Mock<IMinioService>();
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        IReadOnlyCollection<Guid>? deletedIds = null;
        repository.Setup(x => x.GetReportsForCleanup(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new WeeklyAdminReportCleanupCandidate
                {
                    Id = firstId,
                    ReportName = "first.csv",
                    ObjectName = "first.csv"
                },
                new WeeklyAdminReportCleanupCandidate
                {
                    Id = secondId,
                    ReportName = "second.csv",
                    ObjectName = "second.csv"
                }
            ]);
        minioService.Setup(x => x.RemoveFileAsync("weekly-reports", "first.csv", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        minioService.Setup(x => x.RemoveFileAsync("weekly-reports", "second.csv", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("remove failed"));
        repository.Setup(x => x.MarkReportsDeleted(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyCollection<Guid>, DateTime, CancellationToken>((ids, _, _) => deletedIds = ids)
            .Returns(Task.CompletedTask);
        var service = new WeeklyAdminReportStorageService(
            repository.Object,
            csvBuilder.Object,
            minioService.Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 9, 0, 0, DateTimeKind.Utc)));

        await service.CleanupAsync("weekly-reports", 30);

        Assert.NotNull(deletedIds);
        Assert.Single(deletedIds!);
        Assert.Contains(firstId, deletedIds!);
        Assert.DoesNotContain(secondId, deletedIds!);
    }

    private static WeeklyAdminReportProcessingRequest CreateRequest()
    {
        return new WeeklyAdminReportProcessingRequest
        {
            TimeZoneId = "UTC",
            Template = new WeeklyReportTemplate
            {
                ReportsBucketName = "weekly-reports"
            },
            EmailSettings = new EmailSettings()
        };
    }
}
