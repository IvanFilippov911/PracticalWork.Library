using PracticalWork.Library.Services;
using PracticalWork.Tests.Common;

namespace PracticalWork.Library.Tests.Services;

public sealed class WeeklyAdminReportPeriodServiceTests
{
    [Fact]
    public void GetPreviousWeek_WhenCurrentDateIsMidWeek_ReturnsPreviousMondayToSunday()
    {
        var service = new WeeklyAdminReportPeriodService(
            new TestTimeProvider(new DateTime(2026, 5, 27, 10, 0, 0, DateTimeKind.Utc)));

        var result = service.GetPreviousWeek("UTC");

        Assert.Equal(new DateOnly(2026, 5, 18), result.PeriodFrom);
        Assert.Equal(new DateOnly(2026, 5, 24), result.PeriodTo);
        Assert.Equal(TimeZoneInfo.Utc.Id, result.TimeZone.Id);
    }

    [Fact]
    public void GetPreviousWeek_WhenTimeZoneShiftsDate_UsesLocalCalendarBoundaries()
    {
        var service = new WeeklyAdminReportPeriodService(
            new TestTimeProvider(new DateTime(2026, 5, 25, 0, 30, 0, DateTimeKind.Utc)));

        var result = service.GetPreviousWeek("America/Los_Angeles");

        Assert.Equal(new DateOnly(2026, 5, 11), result.PeriodFrom);
        Assert.Equal(new DateOnly(2026, 5, 17), result.PeriodTo);
        Assert.Equal("America/Los_Angeles", result.TimeZone.Id);
    }
}
