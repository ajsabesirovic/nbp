namespace FitJourney.Domain.Entities;

public class SessionSet
{
    public int SetNumber { get; set; }
    public double? Reps { get; set; }
    public double? WeightKg { get; set; }
    public double? DurationSec { get; set; }
    public double? DistanceM { get; set; }
    public int? Rpe { get; set; }
    public bool Completed { get; set; } = true;
}

public class PerformedExercise
{
    public string ExerciseId { get; set; } = default!;
    public string? NameSnapshot { get; set; }
    public string? Type { get; set; }
    public List<SessionSet> Sets { get; set; } = [];
}

public class WorkoutSession
{
    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string? PlanId { get; set; }
    public int? PlanDayNumber { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public List<PerformedExercise> Exercises { get; set; } = [];
    public string? Notes { get; set; }
    public int? Feeling { get; set; }
    public double TotalVolumeKg { get; set; }
    public int CompletedSets { get; set; }
    public int DurationSec { get; set; }
    public DateTime CreatedAt { get; set; }
}
