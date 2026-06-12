using FluentValidation;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Exercises.Commands;
namespace FitJourney.Application.Validators;

public class CreateExerciseRequestValidator : AbstractValidator<CreateExerciseRequest>
{

    internal static readonly string[] ValidTypes =
        ["strength", "cardio", "flexibility", "balance", "weighted", "bodyweight", "endurance", "isometric"];
    public CreateExerciseRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Type).NotEmpty().Must(t => ValidTypes.Contains(t)).WithMessage("Invalid exercise type");
        RuleFor(x => x.PrimaryMuscles).NotEmpty();
        RuleFor(x => x.Difficulty!.Value).InclusiveBetween(1, 5).When(x => x.Difficulty.HasValue);
    }
}

public class CreateExerciseCommandValidator : AbstractValidator<CreateExerciseCommand>
{
    public CreateExerciseCommandValidator() =>
        RuleFor(c => c.Request).NotNull().SetValidator(new CreateExerciseRequestValidator());
}

public class UpdateExerciseCommandValidator : AbstractValidator<UpdateExerciseCommand>
{
    public UpdateExerciseCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Request).NotNull();

        RuleFor(c => c.Request.Name).NotEmpty().MaximumLength(100)
            .When(c => c.Request is not null && c.Request.Name is not null);
        RuleFor(c => c.Request.Type)
            .Must(t => CreateExerciseRequestValidator.ValidTypes.Contains(t!)).WithMessage("Invalid exercise type")
            .When(c => c.Request is not null && c.Request.Type is not null);
        RuleFor(c => c.Request.Difficulty!.Value).InclusiveBetween(1, 5)
            .When(c => c.Request is not null && c.Request.Difficulty.HasValue);
    }
}

public class DeleteExerciseCommandValidator : AbstractValidator<DeleteExerciseCommand>
{
    public DeleteExerciseCommandValidator() => RuleFor(c => c.Id).NotEmpty();
}
