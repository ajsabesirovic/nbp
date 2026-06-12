using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class NotificationDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)] public string UserId { get; set; } = default!;
    [BsonElement("type")] public string Type { get; set; } = default!;
    [BsonElement("title")] public string Title { get; set; } = default!;
    [BsonElement("body")] public string? Body { get; set; }
    [BsonElement("link")] public string? Link { get; set; }
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; }
    [BsonElement("readAt")] public DateTime? ReadAt { get; set; }
}
