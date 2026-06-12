namespace FitJourney.Domain.Entities;

public class TrainerProfile
{
    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public List<string> Certifications { get; set; } = [];
    public string? Specialization { get; set; }
    public double? PricePerPlan { get; set; }
    public string? Bio { get; set; }
    public List<string> ClientIds { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
