using FitJourney.Domain.Enums;

namespace FitJourney.Infrastructure.Services;

internal record ExerciseSeedRow(string Name, ExerciseType Type, List<MuscleGroup> Primary, List<MuscleGroup> Secondary);

internal static class ExerciseSeed
{
    private static List<MuscleGroup> M(params MuscleGroup[] m) => [.. m];

    public static readonly ExerciseSeedRow[] All =
    [
        new("Back Squat",              ExerciseType.weighted,   M(MuscleGroup.quads),                       M(MuscleGroup.glutes, MuscleGroup.core)),
        new("Front Squat",             ExerciseType.weighted,   M(MuscleGroup.quads),                       M(MuscleGroup.core)),
        new("Conventional Deadlift",   ExerciseType.weighted,   M(MuscleGroup.hamstrings, MuscleGroup.back), M(MuscleGroup.glutes)),
        new("Romanian Deadlift",       ExerciseType.weighted,   M(MuscleGroup.hamstrings),                  M(MuscleGroup.glutes, MuscleGroup.back)),
        new("Bench Press",             ExerciseType.weighted,   M(MuscleGroup.chest),                       M(MuscleGroup.triceps, MuscleGroup.shoulders)),
        new("Incline Dumbbell Press",  ExerciseType.weighted,   M(MuscleGroup.chest),                       M(MuscleGroup.shoulders)),
        new("Overhead Press",          ExerciseType.weighted,   M(MuscleGroup.shoulders),                   M(MuscleGroup.triceps, MuscleGroup.core)),
        new("Barbell Row",             ExerciseType.weighted,   M(MuscleGroup.back),                        M(MuscleGroup.biceps)),
        new("Lat Pulldown",            ExerciseType.weighted,   M(MuscleGroup.back),                        M(MuscleGroup.biceps)),
        new("Seated Cable Row",        ExerciseType.weighted,   M(MuscleGroup.back),                        M(MuscleGroup.biceps)),
        new("Bicep Curl",              ExerciseType.weighted,   M(MuscleGroup.biceps),                      M()),
        new("Tricep Pushdown",         ExerciseType.weighted,   M(MuscleGroup.triceps),                     M()),
        new("Leg Press",               ExerciseType.weighted,   M(MuscleGroup.quads),                       M(MuscleGroup.glutes)),
        new("Hip Thrust",              ExerciseType.weighted,   M(MuscleGroup.glutes),                      M(MuscleGroup.hamstrings)),
        new("Walking Lunge",           ExerciseType.weighted,   M(MuscleGroup.quads),                       M(MuscleGroup.glutes)),

        new("Pull-up",                 ExerciseType.bodyweight, M(MuscleGroup.back),                        M(MuscleGroup.biceps)),
        new("Push-up",                 ExerciseType.bodyweight, M(MuscleGroup.chest),                       M(MuscleGroup.triceps, MuscleGroup.shoulders)),
        new("Dips",                    ExerciseType.bodyweight, M(MuscleGroup.chest, MuscleGroup.triceps),  M()),
        new("Bodyweight Squat",        ExerciseType.bodyweight, M(MuscleGroup.quads),                       M(MuscleGroup.glutes)),
        new("Pistol Squat",            ExerciseType.bodyweight, M(MuscleGroup.quads),                       M(MuscleGroup.glutes, MuscleGroup.core)),

        new("Plank",                   ExerciseType.isometric,  M(MuscleGroup.core),                        M()),
        new("Side Plank",              ExerciseType.isometric,  M(MuscleGroup.core),                        M()),
        new("Wall Sit",                ExerciseType.isometric,  M(MuscleGroup.quads),                       M()),
        new("Hollow Hold",             ExerciseType.isometric,  M(MuscleGroup.core),                        M()),

        new("Running",                 ExerciseType.endurance,  M(MuscleGroup.legs),                        M(MuscleGroup.core)),
        new("Cycling",                 ExerciseType.endurance,  M(MuscleGroup.quads),                       M(MuscleGroup.hamstrings)),
        new("Rowing Machine",          ExerciseType.endurance,  M(MuscleGroup.back),                        M(MuscleGroup.legs, MuscleGroup.core)),
        new("Jump Rope",               ExerciseType.endurance,  M(MuscleGroup.calves),                      M(MuscleGroup.shoulders)),
        new("Burpees",                 ExerciseType.endurance,  M(MuscleGroup.legs, MuscleGroup.chest),     M()),

        new("Cat-Cow",                 ExerciseType.bodyweight, M(MuscleGroup.back),                        M()),
        new("World's Greatest Stretch",ExerciseType.bodyweight, M(MuscleGroup.hips),                        M(MuscleGroup.back)),
        new("Pigeon Pose",             ExerciseType.isometric,  M(MuscleGroup.hips),                        M()),
        new("Hamstring Stretch",       ExerciseType.isometric,  M(MuscleGroup.hamstrings),                  M()),
    ];
}
