using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class UserRepository(MongoDbContext db, IMapper mapper) : IUserRepository
{
    public async Task<User?> GetByIdAsync(string id)
    {
        var doc = await db.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
        return doc == null ? null : mapper.Map<User>(doc);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var doc = await db.Users.Find(u => u.Email == email.ToLowerInvariant()).FirstOrDefaultAsync();
        return doc == null ? null : mapper.Map<User>(doc);
    }

    public async Task<User> CreateAsync(User user)
    {
        var doc = mapper.Map<UserDocument>(user);
        doc.Id = string.Empty;
        await db.Users.InsertOneAsync(doc);
        return mapper.Map<User>(doc);
    }

    public async Task<User> UpdateAsync(User user)
    {
        var doc = mapper.Map<UserDocument>(user);
        await db.Users.ReplaceOneAsync(u => u.Id == user.Id, doc);
        return mapper.Map<User>(doc);
    }

    public async Task<(List<User> Items, long Total)> ListAsync(string? search, string? role, int page, int limit)
    {
        var fb = Builders<UserDocument>.Filter;
        var clauses = new List<FilterDefinition<UserDocument>>();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var rx = new BsonRegularExpression(search, "i");
            clauses.Add(fb.Or(fb.Regex(u => u.Name, rx), fb.Regex(u => u.Email, rx)));
        }
        if (!string.IsNullOrWhiteSpace(role))
            clauses.Add(fb.Eq(u => u.Role, role));

        var filter = clauses.Count == 0 ? fb.Empty : fb.And(clauses);
        var total = await db.Users.CountDocumentsAsync(filter);
        var docs = await db.Users.Find(filter)
            .SortBy(u => u.Name)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();
        return (mapper.Map<List<User>>(docs), total);
    }

    public async Task<long> CountAsync(string? role = null)
    {
        var filter = string.IsNullOrWhiteSpace(role)
            ? Builders<UserDocument>.Filter.Empty
            : Builders<UserDocument>.Filter.Eq(u => u.Role, role);
        return await db.Users.CountDocumentsAsync(filter);
    }

    public async Task<long> CountCreatedSinceAsync(DateTime since)
    {
        return await db.Users.CountDocumentsAsync(
            Builders<UserDocument>.Filter.Gte(u => u.CreatedAt, since));
    }

    public async Task<List<User>> GetByRoleAsync(string role)
    {
        var docs = await db.Users
            .Find(Builders<UserDocument>.Filter.Eq(u => u.Role, role))
            .SortBy(u => u.Name)
            .ToListAsync();
        return mapper.Map<List<User>>(docs);
    }
}
