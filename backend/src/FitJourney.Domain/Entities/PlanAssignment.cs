namespace FitJourney.Domain.Entities;

public class PlanAssignment
{
    public string Id { get; set; } = default!;
    public string PlanId { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string AssignedBy { get; set; } = default!;
    public string Status { get; set; } = "active";
    public DateTime AssignedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
