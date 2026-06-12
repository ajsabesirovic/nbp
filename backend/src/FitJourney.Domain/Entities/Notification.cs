namespace FitJourney.Domain.Entities;

public class Notification
{
    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Body { get; set; }
    public string? Link { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
