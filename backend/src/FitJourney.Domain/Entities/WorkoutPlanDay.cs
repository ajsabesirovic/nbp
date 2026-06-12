namespace FitJourney.Domain.Entities;

public class PlanExercise
{
    public string ExerciseId { get; set; } = default!;
    public string? NameSnapshot { get; set; }
    public int Sets { get; set; } = 3;
    public string? Reps { get; set; }
    public int RestSeconds { get; set; } = 90;
    public string? Notes { get; set; }
    public List<string> AlternateExerciseIds { get; set; } = [];
}

public class WorkoutPlanDay
{
    public int DayNumber { get; set; }
    public string Name { get; set; } = default!;
    public List<PlanExercise> Exercises { get; set; } = [];
}
