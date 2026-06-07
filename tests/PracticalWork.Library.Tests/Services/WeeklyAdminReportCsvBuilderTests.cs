using PracticalWork.Library.Models.ReportModels;
using PracticalWork.Library.Services;

namespace PracticalWork.Library.Tests.Services;

public sealed class WeeklyAdminReportCsvBuilderTests
{
    [Fact]
    public void Build_WhenStatisticsProvided_ReturnsCsvWithHeaderAndDataRow()
    {
        var builder = new WeeklyAdminReportCsvBuilder();

        var csv = builder.Build(
            new DateOnly(2026, 5, 19),
            new DateOnly(2026, 5, 25),
            new WeeklyAdminReportStatistics
            {
                NewBooksCount = 2,
                NewReadersCount = 3,
                BorrowedCount = 5,
                ReturnedCount = 4,
                OverdueCount = 1
            });

        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.Equal("ПериодНачало;ПериодКонец;НовыхКниг;НовыхЧитателей;ВыданоКниг;ВозвращеноКниг;ПросроченныхВыдач", lines[0]);
        Assert.Equal("2026-05-19;2026-05-25;2;3;5;4;1", lines[1]);
    }
}
