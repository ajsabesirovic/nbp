namespace FitJourney.Domain.Entities;

public class PersonalRecord
{
    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string ExerciseId { get; set; } = default!;
    public string? ExerciseName { get; set; }
    public string Type { get; set; } = "1rm";
    public double WeightKg { get; set; }
    public int Reps { get; set; }
    public double OneRepMax { get; set; }
    public string? SessionId { get; set; }
    public DateTime AchievedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
