using FitJourney.Domain.Enums;
namespace FitJourney.Domain.Entities;

public class WorkoutPlan
{
    public string Id { get; set; } = default!;
    public string AuthorId { get; set; } = default!;
    public string AuthorName { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int DurationWeeks { get; set; }
    public PlanLevel Level { get; set; }
    public PlanGoal Goal { get; set; }
    public int DaysPerWeek { get; set; }
    public Visibility Visibility { get; set; }
    public string Status { get; set; } = "published";
    public List<WorkoutPlanDay> Days { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
