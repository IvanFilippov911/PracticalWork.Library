using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Library.Abstractions.Services;

namespace PracticalWork.Library.Controllers.Api.v1;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/jobs")]
public sealed class JobsController : Controller
{
    private readonly IJobManagementService _jobManagementService;

    public JobsController(IJobManagementService jobManagementService)
    {
        _jobManagementService = jobManagementService;
    }

    /// <summary>
    /// Получить список настроенных фоновых задач.
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    public IActionResult GetJobs()
    {
        var jobs = _jobManagementService.GetJobs();
        return Ok(jobs);
    }

    /// <summary>
    /// Запустить задачу вручную.
    /// </summary>
    [HttpPost("{jobName}/trigger")]
    [Produces("application/json")]
    [ProducesResponseType(202)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> TriggerJob(string jobName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(jobName))
        {
            return BadRequest("Необходимо указать имя задачи.");
        }

        try
        {
            await _jobManagementService.TriggerAsync(jobName, cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }

        return Accepted(new { Message = $"Задача '{jobName}' поставлена в выполнение." });
    }
}
