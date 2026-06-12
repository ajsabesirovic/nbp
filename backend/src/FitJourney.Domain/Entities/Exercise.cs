using FitJourney.Domain.Enums;
namespace FitJourney.Domain.Entities;

public class Exercise
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public ExerciseType Type { get; set; }
    public List<MuscleGroup> PrimaryMuscles { get; set; } = [];
    public List<MuscleGroup> SecondaryMuscles { get; set; } = [];
    public string? Category { get; set; }
    public string? Equipment { get; set; }
    public int? Difficulty { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public bool IsCustom { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
