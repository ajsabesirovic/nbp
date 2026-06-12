using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<(List<User> Items, long Total)> ListAsync(string? search, string? role, int page, int limit);
    Task<long> CountAsync(string? role = null);
    Task<long> CountCreatedSinceAsync(DateTime since);
    Task<List<User>> GetByRoleAsync(string role);
}
