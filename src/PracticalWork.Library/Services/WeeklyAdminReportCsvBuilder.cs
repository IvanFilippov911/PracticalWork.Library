using System.Text;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Services;

/// <summary>
/// Сборщик CSV для еженедельного административного отчета.
/// </summary>
public sealed class WeeklyAdminReportCsvBuilder : IWeeklyAdminReportCsvBuilder
{
    public string Build(
        DateOnly periodFrom,
        DateOnly periodTo,
        WeeklyAdminReportStatistics statistics)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ПериодНачало;ПериодКонец;НовыхКниг;НовыхЧитателей;ВыданоКниг;ВозвращеноКниг;ПросроченныхВыдач");
        sb.AppendLine($"{periodFrom:yyyy-MM-dd};{periodTo:yyyy-MM-dd};{statistics.NewBooksCount};{statistics.NewReadersCount};{statistics.BorrowedCount};{statistics.ReturnedCount};{statistics.OverdueCount}");
        return sb.ToString();
    }
}
