namespace FitJourney.Domain.Entities;

public class RefreshToken
{
    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public string? ReplacedByToken { get; set; }
    public DateTime CreatedAt { get; set; }
}
