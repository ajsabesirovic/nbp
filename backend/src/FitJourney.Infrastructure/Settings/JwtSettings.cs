namespace FitJourney.Infrastructure.Settings;
public class JwtSettings
{
    public string Secret { get; set; } = default!;
    public int AccessTokenExpiryMinutes { get; set; } = 15;
    public int RefreshTokenExpiryDays { get; set; } = 7;
}
