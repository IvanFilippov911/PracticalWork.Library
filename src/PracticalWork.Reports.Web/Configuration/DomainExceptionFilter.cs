using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PracticalWork.Reports.Web.Configuration;

/// <summary>
/// Фильтр для преобразования доменных исключений в HTTP 400.
/// </summary>
/// <typeparam name="TAppException">Тип доменного исключения.</typeparam>
public class DomainExceptionFilter<TAppException> : IAsyncActionFilter where TAppException : Exception
{
    protected readonly ILogger Logger;

    /// <summary>
    /// Инициализирует фильтр доменных исключений.
    /// </summary>
    /// <param name="logger">Логгер фильтра.</param>
    public DomainExceptionFilter(ILogger<DomainExceptionFilter<TAppException>> logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Выполняет действие и при необходимости преобразует исключение в ответ Bad Request.
    /// </summary>
    /// <param name="context">Контекст выполнения действия.</param>
    /// <param name="next">Следующий делегат конвейера фильтров.</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();
        if (HasException(resultContext))
        {
            TryHandleException(resultContext, resultContext.Exception!);
        }
    }

    private static bool HasException(ActionExecutedContext context) => context.Exception != null && !context.ExceptionHandled;

    protected virtual void TryHandleException(ActionExecutedContext context, Exception exception)
    {
        if (exception is not TAppException)
            return;

        var problemDetails = BuildProblemDetails(exception);

        context.Result = new BadRequestObjectResult(problemDetails);
        context.ExceptionHandled = true;

        Logger.LogError(exception, "Unhandled domain exception. Transformed to Bad request (400).");
    }

    protected static ValidationProblemDetails BuildProblemDetails(Exception exception)
    {
        var exceptionName = exception.GetType().Name;
        var errorMessages = new[] { exception.Message };

        // Используем ValidationProblemDetails, а не базовый или свой тип, т. к. он обвешан атрибутами сериализации
        // и так мы можем гарантировать идентичный ответ и при ошибках валидации, и при доменных исключениях:
        var problemDetails = new ValidationProblemDetails
        {
            Title = "Произошла ошибка во время выполнения запроса.",
            Errors = { { exceptionName, errorMessages } }
        };

        return problemDetails;
    }
}
