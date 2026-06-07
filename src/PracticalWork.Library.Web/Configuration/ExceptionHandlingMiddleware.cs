using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Exceptions;

namespace PracticalWork.Library.Web.Configuration
{
    /// <summary>
    /// Мидлвейр для обработки ошибок
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

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
            Dictionary<string, object> details = null)
        {
            var errorId = Guid.NewGuid().ToString();
            details ??= new Dictionary<string, object>();
            details.Add("errorId", errorId);

            _logger.Log(logLevel, exception, "Error #{errorId}: {errorText}", errorId, errorText);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)responseCode;

            var response = new ProblemDetails
            {
                Title = errorText,
                Instance = null,
                Status = (int)responseCode,
                Type = null,
            };

            foreach (var detail in details)
                response.Extensions.Add(detail.Key, detail.Value);

            var jsonOptions = context.RequestServices.GetRequiredService<IOptions<JsonOptions>>();
           
            var problemDetails = new ProblemDetails
            {
                Title = "error",
                Detail = exception.Message,
                Status = (int)responseCode,
                Type = exception.GetType().FullName,
            };
            
            // await context.Response.WriteAsync(
            //     JsonSerializer.Serialize(response, jsonOptions.Value.JsonSerializerOptions));
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        
        /// <summary>
        /// Обработка исключения <see cref="Exception"/>
        /// </summary>
        /// <param name="context">Контекст запроса ASP.NET</param>
        /// <param name="exception">Исключение</param>
        /// <returns>Задача на обработку запроса ASP.NET</returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorText = exception.Message;
            var logLevel = LogLevel.Error;
            var responseCode = HttpStatusCode.InternalServerError;
            await LogAndReturnAsync(context, exception, errorText, responseCode, logLevel);
        }
        
        /// <summary>
        /// Обработка исключения <see cref="UnauthorizedAccessException"/>
        /// </summary>
        /// <param name="context">Контекст запроса ASP.NET</param>
        /// <param name="exception">Исключение</param>
        /// <returns>Задача на обработку запроса ASP.NET</returns>
        private async Task HandleUnauthorizedAccessExceptionAsync(HttpContext context, UnauthorizedAccessException exception)
        {
            var errorText = "Доступ к запрашиваемому ресурсу ограничен";
            var logLevel = LogLevel.Warning;
            var responseCode = HttpStatusCode.Forbidden;
            await LogAndReturnAsync(context, exception, errorText, responseCode, logLevel);
        }

        /// <summary>
        /// Обработка исключения <see cref="NotFoundException"/>
        /// </summary>
        /// <param name="context">Контекст запроса ASP.NET</param>
        /// <param name="exception">Исключение</param>
        /// <returns>Задача на обработку запроса ASP.NET</returns>
        private async Task HandleEntityNotFoundExceptionAsync(HttpContext context,
            NotFoundException exception)
        {
            var errorText = exception.Message;
            var logLevel = LogLevel.Warning;
            var responseCode = HttpStatusCode.NotFound;
            await LogAndReturnAsync(context, exception, errorText, responseCode, logLevel);
        }
        
        /// <summary>
        /// Обработка исключения <see cref="AppException"/>
        /// </summary>
        /// <param name="context">Контекст запроса ASP.NET</param>
        /// <param name="exception">Исключение</param>
        /// <returns>Задача на обработку запроса ASP.NET</returns>
        private async Task HandleApplicationExceptionBaseAsync(HttpContext context, AppException exception)
        {
            var errorText = exception.Message;
            var logLevel = LogLevel.Warning;
            var responseCode = HttpStatusCode.BadRequest;
            await LogAndReturnAsync(context, exception, errorText, responseCode, logLevel);
        }
    }
}