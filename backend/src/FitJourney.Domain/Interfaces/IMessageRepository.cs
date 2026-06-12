using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IMessageRepository
{
    Task<List<Message>> GetThreadAsync(string userIdA, string userIdB, int limit);
    Task<List<Message>> GetInboxAsync(string userId);
    Task<long> CountUnreadAsync(string userId);
    Task<Message> CreateAsync(Message m);
    Task MarkThreadReadAsync(string userId, string otherUserId);
}
