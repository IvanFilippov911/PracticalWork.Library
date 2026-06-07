using Microsoft.AspNetCore.Mvc;
using Moq;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Enums;
using PracticalWork.Library.Contracts.v1.Reports.Request;
using PracticalWork.Library.Contracts.v1.Reports.Response;
using PracticalWork.Reports.Web.Controllers;

namespace PracticalWork.Reports.Tests.Controllers;

public sealed class ReportControllerTests
{
    [Fact]
    public async Task GetActivityLogs_ReturnsOkWithServiceResponse()
    {
        var reportService = new Mock<IReportService>();
        var response = new PaginationResponse<ActivityLogResponse>
        {
            Entities = [],
            TotalCount = 0,
            PageCount = 0,
            PageNumber = 1,
            PageSize = 20
        };
        reportService.Setup(x => x.ReadSystemActivityLogs(It.IsAny<ActivityLogsPaginationRequest>()))
            .ReturnsAsync(response);
        var controller = new ReportController(reportService.Object);

        var result = await controller.GetActivityLogs(new ActivityLogsPaginationRequest { PageNumber = 1, PageSize = 20, EventTypes = [] });

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task CreateReportCsv_ReturnsOkWithCreatedReport()
    {
        var reportService = new Mock<IReportService>();
        var response = new ReportCreateResponse
        {
            PeriodFrom = new DateOnly(2026, 5, 1),
            PeriodTo = new DateOnly(2026, 5, 7),
            EventTypes = ["book.created"],
            Status = ReportStatus.InProgress
        };
        reportService.Setup(x => x.CreateReport(It.IsAny<ReportCreateRequest>())).ReturnsAsync(response);
        var controller = new ReportController(reportService.Object);

        var result = await controller.CreateReportCsv(new ReportCreateRequest(new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 7), ["book.created"]));

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task GetGeneratedReports_ReturnsOkWithReportList()
    {
        var reportService = new Mock<IReportService>();
        IReadOnlyList<ReportResponse> response =
        [
            new ReportResponse("report.csv", "https://minio/report.csv", new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 7), ["book.created"], DateTime.UtcNow)
        ];
        reportService.Setup(x => x.GetListOfReadyReports()).ReturnsAsync(response);
        var controller = new ReportController(reportService.Object);

        var result = await controller.GetGeneratedReports();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task GetGeneratedReportUrl_ReturnsOkWithUrl()
    {
        var reportService = new Mock<IReportService>();
        reportService.Setup(x => x.GetReportUrl("report.csv")).ReturnsAsync("https://minio/report.csv");
        var controller = new ReportController(reportService.Object);

        var result = await controller.GetGeneratedReportUrl("report.csv");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("https://minio/report.csv", okResult.Value);
    }
}
