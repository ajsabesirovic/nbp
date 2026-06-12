namespace FitJourney.Domain.Entities;

public class ProgressPhoto
{
    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string Url { get; set; } = default!;
    public DateTime TakenAt { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
