using PracticalWork.Library.Contracts.v1.Reports.Request;
using PracticalWork.Reports.Web.Validations;

namespace PracticalWork.Reports.Tests.Validations;

public sealed class ReportCreateRequestValidatorTests
{
    [Fact]
    public void Validate_WhenRequestIsValid_ReturnsSuccess()
    {
        var validator = new ReportCreateRequestValidator();

        var result = validator.Validate(new ReportCreateRequest(
            new DateOnly(2026, 5, 1),
            new DateOnly(2026, 5, 7),
            ["book.created", "reader.created"]));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenEventTypesEmpty_ReturnsFailure()
    {
        var validator = new ReportCreateRequestValidator();

        var result = validator.Validate(new ReportCreateRequest(
            new DateOnly(2026, 5, 1),
            new DateOnly(2026, 5, 7),
            []));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenAnyEventTypeEmpty_ReturnsFailure()
    {
        var validator = new ReportCreateRequestValidator();

        var result = validator.Validate(new ReportCreateRequest(
            new DateOnly(2026, 5, 1),
            new DateOnly(2026, 5, 7),
            ["book.created", string.Empty]));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenDateRangeInvalid_ReturnsFailure()
    {
        var validator = new ReportCreateRequestValidator();

        var result = validator.Validate(new ReportCreateRequest(
            new DateOnly(2026, 5, 8),
            new DateOnly(2026, 5, 7),
            ["book.created"]));

        Assert.False(result.IsValid);
    }
}
