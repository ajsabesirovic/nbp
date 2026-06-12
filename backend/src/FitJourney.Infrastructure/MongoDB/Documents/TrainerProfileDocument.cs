using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class TrainerProfileDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = default!;
    [BsonElement("certifications")] public List<string> Certifications { get; set; } = [];
    [BsonElement("specialization")] public string? Specialization { get; set; }
    [BsonElement("pricePerPlan")] public double? PricePerPlan { get; set; }
    [BsonElement("bio")] public string? Bio { get; set; }
    [BsonElement("clientIds"), BsonRepresentation(BsonType.ObjectId)]
    public List<string> ClientIds { get; set; } = [];
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; }
    [BsonElement("updatedAt")] public DateTime UpdatedAt { get; set; }
}
