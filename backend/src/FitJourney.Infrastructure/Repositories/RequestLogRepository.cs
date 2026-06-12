using AutoMapper;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class RequestLogRepository(MongoDbContext db, IMapper mapper) : IRequestLogRepository
{
    public async Task InsertAsync(RequestLog log)
    {
        var doc = mapper.Map<RequestLogDocument>(log);
        doc.Id = string.Empty;
        await db.RequestLogs.InsertOneAsync(doc);
    }

    public async Task<(List<RequestLog> Items, long Total)> RecentAsync(int page, int limit)
    {
        var filter = FilterDefinition<RequestLogDocument>.Empty;
        var total = await db.RequestLogs.CountDocumentsAsync(filter);
        var docs = await db.RequestLogs
            .Find(filter)
            .SortByDescending(d => d.Timestamp)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();
        return (mapper.Map<List<RequestLog>>(docs), total);
    }

    public async Task<(List<RequestLog> Items, long Total)> SlowAsync(long thresholdMs, int page, int limit)
    {
        var filter = Builders<RequestLogDocument>.Filter.Gte(d => d.DurationMs, thresholdMs);
        var total = await db.RequestLogs.CountDocumentsAsync(filter);
        var docs = await db.RequestLogs
            .Find(filter)
            .SortByDescending(d => d.Timestamp)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();
        return (mapper.Map<List<RequestLog>>(docs), total);
    }
}
