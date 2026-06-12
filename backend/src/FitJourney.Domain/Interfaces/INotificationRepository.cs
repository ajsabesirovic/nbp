using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserAsync(string userId, int limit);
    Task<long> CountUnreadAsync(string userId);

    Task<bool> ExistsByTypeSinceAsync(string userId, string type, DateTime since);
    Task<Notification> CreateAsync(Notification n);
    Task MarkAllReadAsync(string userId);
    Task<bool> MarkReadAsync(string id, string userId);
}
