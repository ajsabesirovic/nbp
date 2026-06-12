using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class MessageDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("fromUserId"), BsonRepresentation(BsonType.ObjectId)] public string FromUserId { get; set; } = default!;
    [BsonElement("toUserId"), BsonRepresentation(BsonType.ObjectId)] public string ToUserId { get; set; } = default!;
    [BsonElement("fromName")] public string FromName { get; set; } = default!;
    [BsonElement("toName")] public string ToName { get; set; } = default!;
    [BsonElement("body")] public string Body { get; set; } = default!;
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; }
    [BsonElement("readAt")] public DateTime? ReadAt { get; set; }
}
