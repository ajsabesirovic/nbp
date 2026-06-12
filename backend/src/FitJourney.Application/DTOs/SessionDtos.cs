namespace FitJourney.Application.DTOs;

public record SessionSetDto(int SetNumber, double? Reps, double? WeightKg, double? DurationSec, double? DistanceM, int? Rpe, bool Completed);
public record PerformedExerciseDto(string ExerciseId, string? NameSnapshot, string? Type, List<SessionSetDto> Sets);
public record SessionDto(string Id, string UserId, string? PlanId, DateTime StartedAt, DateTime? EndedAt, List<PerformedExerciseDto> Exercises, string? Notes, int? Feeling, double TotalVolumeKg, int CompletedSets, int DurationSec, DateTime CreatedAt);
public record CreateSessionRequest(string? PlanId, DateTime StartedAt, DateTime EndedAt, List<PerformedExerciseDto> Exercises, string? Notes, int? Feeling);
