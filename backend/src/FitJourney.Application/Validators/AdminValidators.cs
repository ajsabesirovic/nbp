using FluentValidation;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Admin.Commands;
namespace FitJourney.Application.Validators;

public class SetUserRoleRequestValidator : AbstractValidator<SetUserRoleRequest>
{
    internal static readonly string[] Allowed = ["user", "trainer", "admin"];

    public SetUserRoleRequestValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => Allowed.Contains(r, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Role must be one of: user, trainer, admin.");
    }
}

public class ModeratePlanRequestValidator : AbstractValidator<ModeratePlanRequest>
{
    internal static readonly string[] Allowed = ["draft", "published", "archived"];

    public ModeratePlanRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(s => Allowed.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status must be one of: draft, published, archived.");
    }
}

public class SetUserRoleCommandValidator : AbstractValidator<SetUserRoleCommand>
{
    public SetUserRoleCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Role)
            .NotEmpty()
            .Must(r => SetUserRoleRequestValidator.Allowed.Contains(r, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Role must be one of: user, trainer, admin.");
    }
}

public class ModeratePlanCommandValidator : AbstractValidator<ModeratePlanCommand>
{
    public ModeratePlanCommandValidator()
    {
        RuleFor(c => c.PlanId).NotEmpty();
        RuleFor(c => c.Status)
            .NotEmpty()
            .Must(s => ModeratePlanRequestValidator.Allowed.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status must be one of: draft, published, archived.");
    }
}
