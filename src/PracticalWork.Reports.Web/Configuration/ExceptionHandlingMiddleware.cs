using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Contracts.Exceptions;

namespace PracticalWork.Reports.Web.Configuration;

/// <summary>
/// Middleware централизованной обработки необработанных исключений.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Инициализирует middleware обработки исключений.
    /// </summary>
    /// <param name="next">Следующий middleware конвейера.</param>
    /// <param name="logger">Логгер middleware.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Выполняет следующий middleware и обрабатывает необработанные исключения.
    /// </summary>
    /// <param name="context">HTTP-контекст запроса.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await ProcessExceptionAsync(context, ex);
        }
    }

    private async Task ProcessExceptionAsync(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case UnauthorizedAccessException accessException:
                await HandleUnauthorizedAccessExceptionAsync(context, accessException);
                break;
            case NotFoundException notFoundException:
                await HandleEntityNotFoundExceptionAsync(context, notFoundException);
                break;
            case AppException applicationException:
                await HandleApplicationExceptionBaseAsync(context, applicationException);
                break;
            default:
                await HandleExceptionAsync(context, exception);
                break;
        }
    }

    private async Task LogAndReturnAsync(
        HttpContext context,
        Exception exception,
        string errorText,
        HttpStatusCode responseCode,
        LogLevel logLevel = LogLevel.Error,
        Dictionary<string, object>? details = null)
    {
        var errorId = Guid.NewGuid().ToString();
        details ??= new Dictionary<string, object>();
        details.Add("errorId", errorId);

        _logger.Log(logLevel, exception, "Error #{errorId}: {errorText}", errorId, errorText);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)responseCode;

        var jsonOptions = context.RequestServices.GetRequiredService<IOptions<JsonOptions>>();

        var problemDetails = new ProblemDetails
        {
            Title = "error",
            Detail = exception.Message,
            Status = (int)responseCode,
            Type = exception.GetType().FullName,
        };

        await context.Response.WriteAsJsonAsync(problemDetails, jsonOptions.Value.JsonSerializerOptions);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorText = exception.Message;
        var logLevel = LogLevel.Error;
        var responseCode = HttpStatusCode.InternalServerError;
        await LogAndReturnAsync(context, exception, errorText, responseCode, logLevel);
    }

    private async Task HandleUnauthorizedAccessExceptionAsync(HttpContext context, UnauthorizedAccessException exception)
    {
        var errorText = "Доступ к запрашиваемому ресурсу ограничен";
        var logLevel = LogLevel.Warning;
        var responseCode = HttpStatusCode.Forbidden;
        await LogAndReturnAsync(context, exception, errorText, responseCode, logLevel);
    }

    private async Task HandleEntityNotFoundExceptionAsync(HttpContext context, NotFoundException exception)
    {
        var errorText = exception.Message;
        var logLevel = LogLevel.Warning;
        var responseCode = HttpStatusCode.NotFound;
        await LogAndReturnAsync(context, exception, errorText, responseCode, logLevel);
    }

    private async Task HandleApplicationExceptionBaseAsync(HttpContext context, AppException exception)
    {
        var errorText = exception.Message;
        var logLevel = LogLevel.Warning;
        var responseCode = HttpStatusCode.BadRequest;
        await LogAndReturnAsync(context, exception, errorText, responseCode, logLevel);
    }
}
