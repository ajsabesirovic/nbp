namespace FitJourney.Domain.Entities;

public class RequestLog
{
    public string Id { get; set; } = default!;
    public string Method { get; set; } = default!;
    public string Path { get; set; } = default!;
    public int StatusCode { get; set; }
    public long DurationMs { get; set; }
    public string? UserId { get; set; }
    public string RequestId { get; set; } = default!;
    public bool SlowRequest { get; set; }
    public DateTime Timestamp { get; set; }
}
