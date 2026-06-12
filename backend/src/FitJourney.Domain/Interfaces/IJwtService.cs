using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string? ValidateAccessToken(string token);
}
