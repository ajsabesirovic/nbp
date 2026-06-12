namespace FitJourney.Domain.Entities;

public class Message
{
    public string Id { get; set; } = default!;
    public string FromUserId { get; set; } = default!;
    public string ToUserId { get; set; } = default!;
    public string FromName { get; set; } = default!;
    public string ToName { get; set; } = default!;
    public string Body { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
