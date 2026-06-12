using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task RevokeAsync(string token, string? replacedBy = null);
    Task RevokeAllForUserAsync(string userId);
}
