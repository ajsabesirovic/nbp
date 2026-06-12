using AutoMapper;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class RefreshTokenRepository(MongoDbContext db, IMapper mapper) : IRefreshTokenRepository
{
    public async Task<RefreshToken> CreateAsync(RefreshToken token)
    {
        var doc = mapper.Map<RefreshTokenDocument>(token);
        doc.Id = string.Empty;
        await db.RefreshTokens.InsertOneAsync(doc);
        return mapper.Map<RefreshToken>(doc);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        var doc = await db.RefreshTokens.Find(t => t.Token == token).FirstOrDefaultAsync();
        return doc == null ? null : mapper.Map<RefreshToken>(doc);
    }

    public async Task RevokeAsync(string token, string? replacedBy = null)
    {
        var update = Builders<RefreshTokenDocument>.Update
            .Set(t => t.IsRevoked, true)
            .Set(t => t.ReplacedByToken, replacedBy);
        await db.RefreshTokens.UpdateOneAsync(t => t.Token == token, update);
    }

    public async Task RevokeAllForUserAsync(string userId)
    {
        var update = Builders<RefreshTokenDocument>.Update.Set(t => t.IsRevoked, true);
        await db.RefreshTokens.UpdateManyAsync(t => t.UserId == userId && !t.IsRevoked, update);
    }
}
