using FluentValidation;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Sessions.Commands;
namespace FitJourney.Application.Validators;

public class CreateSessionRequestValidator : AbstractValidator<CreateSessionRequest>
{
    public CreateSessionRequestValidator()
    {
        RuleFor(x => x.StartedAt).NotEmpty();
        RuleFor(x => x.EndedAt).NotEmpty().GreaterThan(x => x.StartedAt);
        RuleFor(x => x.Exercises).NotEmpty().WithMessage("At least one exercise required");
    }
}

public class LogSessionCommandValidator : AbstractValidator<LogSessionCommand>
{
    public LogSessionCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Request).NotNull().SetValidator(new CreateSessionRequestValidator());
    }
}

public class DeleteSessionCommandValidator : AbstractValidator<DeleteSessionCommand>
{
    public DeleteSessionCommandValidator() => RuleFor(c => c.Id).NotEmpty();
}
