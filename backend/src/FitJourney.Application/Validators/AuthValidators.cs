using FluentValidation;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Auth.Commands;
namespace FitJourney.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator() =>
        RuleFor(c => c.Request).NotNull().SetValidator(new RegisterRequestValidator());
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator() =>
        RuleFor(c => c.Request).NotNull().SetValidator(new LoginRequestValidator());
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator() => RuleFor(c => c.Token).NotEmpty();
}

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator() => RuleFor(c => c.Token).NotEmpty();
}
