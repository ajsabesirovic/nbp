namespace FitJourney.Application.DTOs;

public record BodyMeasurementDto(
    string Id,
    string UserId,
    DateTime RecordedAt,
    double? WeightKg,
    double? WaistCm,
    double? ChestCm,
    double? ArmCm,
    double? ThighCm,
    double? BodyFatPct,
    string? Note,
    DateTime CreatedAt);

public record CreateBodyMeasurementRequest(
    DateTime? RecordedAt,
    double? WeightKg,
    double? WaistCm,
    double? ChestCm,
    double? ArmCm,
    double? ThighCm,
    double? BodyFatPct,
    string? Note);
