using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class RefreshTokenDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)] public string UserId { get; set; } = default!;
    [BsonElement("token")] public string Token { get; set; } = default!;
    [BsonElement("expiresAt")] public DateTime ExpiresAt { get; set; }
    [BsonElement("isRevoked")] public bool IsRevoked { get; set; }
    [BsonElement("replacedByToken")] public string? ReplacedByToken { get; set; }
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; }
}
