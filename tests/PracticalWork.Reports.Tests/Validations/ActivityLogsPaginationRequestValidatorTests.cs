using PracticalWork.Library.Contracts.v1.Reports.Request;
using PracticalWork.Reports.Web.Validations;

namespace PracticalWork.Reports.Tests.Validations;

public sealed class ActivityLogsPaginationRequestValidatorTests
{
    [Fact]
    public void Validate_WhenRequestIsValid_ReturnsSuccess()
    {
        var validator = new ActivityLogsPaginationRequestValidator();

        var result = validator.Validate(new ActivityLogsPaginationRequest
        {
            PageNumber = 1,
            PageSize = 100,
            EventDateFrom = new DateOnly(2026, 5, 1),
            EventDateTo = new DateOnly(2026, 5, 7),
            EventTypes = ["book.created", "reader.created"]
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenPageSizeExceedsLimit_ReturnsFailure()
    {
        var validator = new ActivityLogsPaginationRequestValidator();

        var result = validator.Validate(new ActivityLogsPaginationRequest
        {
            PageNumber = 1,
            PageSize = 501,
            EventTypes = ["book.created"]
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenDateRangeInvalid_ReturnsFailure()
    {
        var validator = new ActivityLogsPaginationRequestValidator();

        var result = validator.Validate(new ActivityLogsPaginationRequest
        {
            PageNumber = 1,
            PageSize = 20,
            EventDateFrom = new DateOnly(2026, 5, 8),
            EventDateTo = new DateOnly(2026, 5, 7),
            EventTypes = ["book.created"]
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenEventTypeEmpty_ReturnsFailure()
    {
        var validator = new ActivityLogsPaginationRequestValidator();

        var result = validator.Validate(new ActivityLogsPaginationRequest
        {
            PageNumber = 1,
            PageSize = 20,
            EventTypes = [string.Empty]
        });

        Assert.False(result.IsValid);
    }
}
