using FluentValidation;
using PracticalWork.Library.Contracts.v1.Reports.Request;

namespace PracticalWork.Reports.Web.Validations;

/// <summary>
/// Валидатор запроса на создание отчета.
/// </summary>
public sealed class ReportCreateRequestValidator : AbstractValidator<ReportCreateRequest>
{
    /// <summary>
    /// Инициализирует правила валидации запроса на создание отчета.
    /// </summary>
    public ReportCreateRequestValidator()
    {
        RuleFor(x => x.EventTypes)
            .NotEmpty()
            .WithMessage("Нужно указать хотя бы один тип события.");

        RuleForEach(x => x.EventTypes)
            .NotEmpty()
            .WithMessage("Тип события не может быть пустым.");

        RuleFor(x => x)
            .Must(x => x.PeriodFrom <= x.PeriodTo)
            .WithMessage("Дата начала периода не может быть больше даты окончания.");
    }
}
