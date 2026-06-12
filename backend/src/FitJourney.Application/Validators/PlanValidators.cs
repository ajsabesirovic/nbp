using FluentValidation;
using FitJourney.Application.Features.Plans.Commands;
namespace FitJourney.Application.Validators;

public class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanCommandValidator()
    {
        RuleFor(c => c.AuthorId).NotEmpty();
        RuleFor(c => c.Request).NotNull();
        RuleFor(c => c.Request.Name).NotEmpty().MaximumLength(150)
            .When(c => c.Request is not null);
        RuleFor(c => c.Request.DurationWeeks).GreaterThan(0)
            .When(c => c.Request is not null);
        RuleFor(c => c.Request.DaysPerWeek).InclusiveBetween(1, 7)
            .When(c => c.Request is not null);
    }
}

public class UpdatePlanCommandValidator : AbstractValidator<UpdatePlanCommand>
{
    public UpdatePlanCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Request).NotNull();

        RuleFor(c => c.Request.Name).NotEmpty().MaximumLength(150)
            .When(c => c.Request is not null && c.Request.Name is not null);
        RuleFor(c => c.Request.DurationWeeks!.Value).GreaterThan(0)
            .When(c => c.Request is not null && c.Request.DurationWeeks.HasValue);
        RuleFor(c => c.Request.DaysPerWeek!.Value).InclusiveBetween(1, 7)
            .When(c => c.Request is not null && c.Request.DaysPerWeek.HasValue);
    }
}

public class DeletePlanCommandValidator : AbstractValidator<DeletePlanCommand>
{
    public DeletePlanCommandValidator() => RuleFor(c => c.Id).NotEmpty();
}
