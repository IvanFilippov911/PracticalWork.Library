using FluentValidation;
using PracticalWork.Library.Contracts.v1.Reports.Request;

namespace PracticalWork.Reports.Web.Validations;

/// <summary>
/// Валидатор запроса на получение журнала активности.
/// </summary>
public sealed class ActivityLogsPaginationRequestValidator : AbstractValidator<ActivityLogsPaginationRequest>
{
    /// <summary>
    /// Инициализирует правила валидации параметров журнала активности.
    /// </summary>
    public ActivityLogsPaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Номер страницы должен быть больше 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 500)
            .WithMessage("Размер страницы должен быть в диапазоне от 1 до 500.");

        RuleFor(x => x)
            .Must(x => x.EventDateFrom is null || x.EventDateTo is null || x.EventDateFrom <= x.EventDateTo)
            .WithMessage("Дата начала периода не может быть больше даты окончания.");

        RuleForEach(x => x.EventTypes)
            .NotEmpty()
            .WithMessage("Тип события не может быть пустым.");
    }
}
