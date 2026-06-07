using FluentValidation;
using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Contracts.v1.Reader.Request;

namespace PracticalWork.Library.Controllers.Validations.v1;

/// <summary>
/// Валидатор запроса на создание читателя с использованием FluentValidation
/// </summary>
public class CreateReaderRequestValidator: AbstractValidator<CreateReaderRequest>
{
    public CreateReaderRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Номер телефона обязателен")
            .MaximumLength(12).WithMessage("Номер телефона не должен превышать 12 символов")
            .Matches("^\\+?\\d{1,3}[\\s-]?\\(?\\d{3}\\)?[\\s-]?\\d{3}[\\s-]?\\d{2}[\\s-]?\\d{2}$")
            .WithMessage("Неправильный формат телефона");
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("ФИО обязательно").NotNull().WithMessage("ФИО обязательно")
            .MaximumLength(50).WithMessage("Имя не должно превышать 50 символов");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Неправильный формат email");
        RuleFor(x => x.ExpiryDate)
            .NotEmpty().WithMessage("Дата окончания карточки обязательна")
            .GreaterThan(DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime));
    }
}
