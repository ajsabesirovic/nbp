using AutoMapper;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class MessageRepository(MongoDbContext db, IMapper mapper) : IMessageRepository
{
    public async Task<List<Message>> GetThreadAsync(string a, string b, int limit)
    {
        var fb = Builders<MessageDocument>.Filter;
        var filter = fb.Or(
            fb.And(fb.Eq(m => m.FromUserId, a), fb.Eq(m => m.ToUserId, b)),
            fb.And(fb.Eq(m => m.FromUserId, b), fb.Eq(m => m.ToUserId, a))
        );
        var docs = await db.Messages.Find(filter).SortBy(m => m.CreatedAt).Limit(limit).ToListAsync();
        return mapper.Map<List<Message>>(docs);
    }

    public async Task<List<Message>> GetInboxAsync(string userId)
    {
        var fb = Builders<MessageDocument>.Filter;
        var filter = fb.Or(fb.Eq(m => m.FromUserId, userId), fb.Eq(m => m.ToUserId, userId));
        var docs = await db.Messages.Find(filter).SortByDescending(m => m.CreatedAt).Limit(200).ToListAsync();
        return mapper.Map<List<Message>>(docs);
    }

    public async Task<long> CountUnreadAsync(string userId)
    {
        var fb = Builders<MessageDocument>.Filter;
        return await db.Messages.CountDocumentsAsync(fb.And(
            fb.Eq(m => m.ToUserId, userId),
            fb.Eq(m => m.ReadAt, (DateTime?)null)));
    }

    public async Task<Message> CreateAsync(Message m)
    {
        var doc = mapper.Map<MessageDocument>(m);
        doc.Id = string.Empty;
        await db.Messages.InsertOneAsync(doc);
        return mapper.Map<Message>(doc);
    }

    public async Task MarkThreadReadAsync(string userId, string otherUserId)
    {
        var fb = Builders<MessageDocument>.Filter;
        var update = Builders<MessageDocument>.Update.Set(m => m.ReadAt, DateTime.UtcNow);
        await db.Messages.UpdateManyAsync(fb.And(
            fb.Eq(m => m.ToUserId, userId),
            fb.Eq(m => m.FromUserId, otherUserId),
            fb.Eq(m => m.ReadAt, (DateTime?)null)), update);
    }
}
