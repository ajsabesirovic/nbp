namespace FitJourney.Domain.Entities;

public class BodyMeasurement
{
    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public DateTime Date { get; set; }
    public double? WeightKg { get; set; }
    public double? WaistCm { get; set; }
    public double? ChestCm { get; set; }
    public double? ArmCm { get; set; }
    public double? ThighCm { get; set; }
    public double? BodyFatPct { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
