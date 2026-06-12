using FluentValidation;
using FitJourney.Application.Features.Profile.Commands;
namespace FitJourney.Application.Validators;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Request).NotNull();
        RuleFor(c => c.Request.Name).MaximumLength(100)
            .When(c => c.Request is not null && c.Request.Name is not null);
    }
}

public class SetActivePlanCommandValidator : AbstractValidator<SetActivePlanCommand>
{

    public SetActivePlanCommandValidator() => RuleFor(c => c.UserId).NotEmpty();
}
