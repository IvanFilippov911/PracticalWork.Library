using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using PracticalWork.Reports.Web.Validations;

namespace PracticalWork.Reports.Web;

/// <summary>
/// Расширения для настройки MVC, валидации и версионирования API сервиса отчетов.
/// </summary>
public static class MvcSetupExtensions
{
    /// <summary>
    /// Регистрирует контроллеры, FluentValidation и API versioning.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    /// <returns>Конфигуратор MVC.</returns>
    public static IMvcBuilder AddReportsApi(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ReportCreateRequestValidator>();
        services.AddFluentValidationAutoValidation();

        services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1.0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services.AddControllers();
    }
}
