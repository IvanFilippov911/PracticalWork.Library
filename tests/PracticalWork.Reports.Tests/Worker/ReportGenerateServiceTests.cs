using System.Text;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.Contracts.Models.ReportModels;
using PracticalWork.Reports.Worker.Services;
using PracticalWork.Tests.Common;

namespace PracticalWork.Reports.Tests.Worker;

public sealed class ReportGenerateServiceTests
{
    [Fact]
    public async Task GenerateReport_WhenSingleLog_CreatesCsvWithHeaderAndRow()
    {
        var service = new ReportGenerateService(new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));
        IReadOnlyList<ActivityLog> logs =
        [
            new ActivityLog
            {
                EventType = "book.created",
                EventDate = new DateTime(2026, 5, 20, 8, 0, 0, DateTimeKind.Utc),
                Event = new BookCreatedEvent(Guid.NewGuid(), "Book", "Fiction", ["Author"], 2024)
            }
        ];

        var result = service.GenerateReport(Guid.Parse("11111111-1111-1111-1111-111111111111"), logs);
        using var reader = new StreamReader(result.Content, Encoding.UTF8, leaveOpen: true);
        result.Content.Position = 0;
        var csv = await reader.ReadToEndAsync();

        Assert.Equal("text/csv", result.ContentType);
        Assert.Equal("2026/5/11111111-1111-1111-1111-111111111111.csv", result.FileName);
        Assert.Contains("EventType;EventDate;Metadata", csv);
        Assert.Contains("book.created", csv);
    }

    [Fact]
    public async Task GenerateReport_WhenManyLogs_WritesAllRows()
    {
        var service = new ReportGenerateService(new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));
        IReadOnlyList<ActivityLog> logs =
        [
            new ActivityLog
            {
                EventType = "book.created",
                EventDate = new DateTime(2026, 5, 20, 8, 0, 0, DateTimeKind.Utc),
                Event = new BookCreatedEvent(Guid.NewGuid(), "Book 1", "Fiction", ["Author"], 2024)
            },
            new ActivityLog
            {
                EventType = "book.created",
                EventDate = new DateTime(2026, 5, 21, 8, 0, 0, DateTimeKind.Utc),
                Event = new BookCreatedEvent(Guid.NewGuid(), "Book 2", "Fiction", ["Author"], 2025)
            }
        ];

        var result = service.GenerateReport(Guid.NewGuid(), logs);
        using var reader = new StreamReader(result.Content, Encoding.UTF8, leaveOpen: true);
        result.Content.Position = 0;
        var csv = await reader.ReadToEndAsync();
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(3, lines.Length);
    }
}
