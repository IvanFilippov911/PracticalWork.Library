using FluentValidation;
using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Contracts.v1.Reader.Request;

namespace PracticalWork.Library.Controllers.Validations.v1;

/// <summary>
/// Валидатор запроса на продление срока действия читательского билета с использованием FluentValidation
/// </summary>
public class ExtendReaderExpiryDateRequestValidator: 
    AbstractValidator<ExtendReaderExpiryDateRequest>
{
    public ExtendReaderExpiryDateRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Дата продления карточки обязательна")
            .GreaterThan(DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime));
    }
}
