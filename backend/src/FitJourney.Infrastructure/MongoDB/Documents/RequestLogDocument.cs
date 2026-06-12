using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class RequestLogDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("method")] public string Method { get; set; } = default!;
    [BsonElement("path")] public string Path { get; set; } = default!;
    [BsonElement("statusCode")] public int StatusCode { get; set; }
    [BsonElement("durationMs")] public long DurationMs { get; set; }
    [BsonElement("userId")] public string? UserId { get; set; }
    [BsonElement("requestId")] public string RequestId { get; set; } = default!;
    [BsonElement("slowRequest")] public bool SlowRequest { get; set; }
    [BsonElement("ts")] public DateTime Timestamp { get; set; }
}
