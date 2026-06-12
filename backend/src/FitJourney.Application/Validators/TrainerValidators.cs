using FluentValidation;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Trainer.Commands;
namespace FitJourney.Application.Validators;

public class AssignPlanRequestValidator : AbstractValidator<AssignPlanRequest>
{
    public AssignPlanRequestValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
    }
}

public class ManageClientRequestValidator : AbstractValidator<ManageClientRequest>
{
    public ManageClientRequestValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
    }
}

public class UpdateTrainerProfileRequestValidator : AbstractValidator<UpdateTrainerProfileRequest>
{
    public UpdateTrainerProfileRequestValidator()
    {
        RuleFor(x => x.PricePerPlan).GreaterThanOrEqualTo(0).When(x => x.PricePerPlan.HasValue);
        RuleFor(x => x.Bio).MaximumLength(2000).When(x => x.Bio != null);
        RuleFor(x => x.Specialization).MaximumLength(200).When(x => x.Specialization != null);
    }
}

public class AddClientCommandValidator : AbstractValidator<AddClientCommand>
{
    public AddClientCommandValidator()
    {
        RuleFor(c => c.TrainerUserId).NotEmpty();
        RuleFor(c => c.ClientUserId).NotEmpty();
    }
}

public class RemoveClientCommandValidator : AbstractValidator<RemoveClientCommand>
{
    public RemoveClientCommandValidator()
    {
        RuleFor(c => c.TrainerUserId).NotEmpty();
        RuleFor(c => c.ClientUserId).NotEmpty();
    }
}

public class AssignPlanToClientCommandValidator : AbstractValidator<AssignPlanToClientCommand>
{
    public AssignPlanToClientCommandValidator()
    {
        RuleFor(c => c.TrainerUserId).NotEmpty();
        RuleFor(c => c.PlanId).NotEmpty();
        RuleFor(c => c.ClientUserId).NotEmpty();
    }
}

public class UnassignPlanFromClientCommandValidator : AbstractValidator<UnassignPlanFromClientCommand>
{
    public UnassignPlanFromClientCommandValidator()
    {
        RuleFor(c => c.TrainerUserId).NotEmpty();
        RuleFor(c => c.PlanId).NotEmpty();
        RuleFor(c => c.ClientUserId).NotEmpty();
    }
}

public class UpdateTrainerProfileCommandValidator : AbstractValidator<UpdateTrainerProfileCommand>
{
    public UpdateTrainerProfileCommandValidator()
    {
        RuleFor(c => c.TrainerUserId).NotEmpty();
        RuleFor(c => c.Request).NotNull().SetValidator(new UpdateTrainerProfileRequestValidator());
    }
}

public class CreateClientMeasurementCommandValidator : AbstractValidator<CreateClientMeasurementCommand>
{
    public CreateClientMeasurementCommandValidator()
    {
        RuleFor(c => c.TrainerUserId).NotEmpty();
        RuleFor(c => c.ClientId).NotEmpty();
        RuleFor(c => c.Request).NotNull();
        RuleFor(c => c.Request.WeightKg).GreaterThan(0)
            .When(c => c.Request is not null && c.Request.WeightKg.HasValue);
    }
}
