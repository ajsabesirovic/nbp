using AutoMapper;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class NotificationRepository(MongoDbContext db, IMapper mapper) : INotificationRepository
{
    public async Task<List<Notification>> GetByUserAsync(string userId, int limit)
    {
        var docs = await db.Notifications
            .Find(n => n.UserId == userId)
            .SortByDescending(n => n.CreatedAt)
            .Limit(limit)
            .ToListAsync();
        return mapper.Map<List<Notification>>(docs);
    }

    public async Task<long> CountUnreadAsync(string userId)
    {
        var fb = Builders<NotificationDocument>.Filter;
        return await db.Notifications.CountDocumentsAsync(fb.And(
            fb.Eq(n => n.UserId, userId),
            fb.Eq(n => n.ReadAt, (DateTime?)null)));
    }

    public async Task<bool> ExistsByTypeSinceAsync(string userId, string type, DateTime since)
    {
        var fb = Builders<NotificationDocument>.Filter;
        return await db.Notifications.Find(fb.And(
            fb.Eq(n => n.UserId, userId),
            fb.Eq(n => n.Type, type),
            fb.Gte(n => n.CreatedAt, since))).AnyAsync();
    }

    public async Task<Notification> CreateAsync(Notification n)
    {
        var doc = mapper.Map<NotificationDocument>(n);
        doc.Id = string.Empty;
        await db.Notifications.InsertOneAsync(doc);
        return mapper.Map<Notification>(doc);
    }

    public async Task MarkAllReadAsync(string userId)
    {
        var fb = Builders<NotificationDocument>.Filter;
        var update = Builders<NotificationDocument>.Update.Set(n => n.ReadAt, DateTime.UtcNow);
        await db.Notifications.UpdateManyAsync(fb.And(
            fb.Eq(n => n.UserId, userId),
            fb.Eq(n => n.ReadAt, (DateTime?)null)), update);
    }

    public async Task<bool> MarkReadAsync(string id, string userId)
    {
        var fb = Builders<NotificationDocument>.Filter;
        var update = Builders<NotificationDocument>.Update.Set(n => n.ReadAt, DateTime.UtcNow);
        var result = await db.Notifications.UpdateOneAsync(
            fb.And(fb.Eq(n => n.Id, id), fb.Eq(n => n.UserId, userId)), update);
        return result.ModifiedCount > 0;
    }
}
