using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class BodyMeasurementDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)] public string UserId { get; set; } = default!;
    [BsonElement("date")] public DateTime Date { get; set; }
    [BsonElement("weightKg")] public double? WeightKg { get; set; }
    [BsonElement("waistCm")] public double? WaistCm { get; set; }
    [BsonElement("chestCm")] public double? ChestCm { get; set; }
    [BsonElement("armCm")] public double? ArmCm { get; set; }
    [BsonElement("thighCm")] public double? ThighCm { get; set; }
    [BsonElement("bodyFatPct")] public double? BodyFatPct { get; set; }
    [BsonElement("note")] public string? Note { get; set; }
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; }
}
