using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class ProgressPhotoDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)] public string UserId { get; set; } = default!;
    [BsonElement("url")] public string Url { get; set; } = default!;
    [BsonElement("takenAt")] public DateTime TakenAt { get; set; }
    [BsonElement("note")] public string? Note { get; set; }
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; }
}
