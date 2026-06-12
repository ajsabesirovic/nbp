namespace FitJourney.Application.DTOs;

public record PlanExerciseDto(string ExerciseId, string? NameSnapshot, int Sets, string? Reps, int RestSeconds, string? Notes, List<string>? AlternateExerciseIds = null);
public record PlanDayDto(int DayNumber, string Name, List<PlanExerciseDto> Exercises);
public record PlanDto(string Id, string AuthorId, string AuthorName, string Name, string? Description, int DurationWeeks, string Level, string Goal, int DaysPerWeek, string Visibility, string Status, List<PlanDayDto> Days, DateTime CreatedAt, DateTime UpdatedAt);
public record CreatePlanRequest(string Name, string? Description, int DurationWeeks, string Level, string Goal, int DaysPerWeek, string Visibility, string Status, List<PlanDayDto> Days);
public record UpdatePlanRequest(string? Name, string? Description, int? DurationWeeks, string? Level, string? Goal, int? DaysPerWeek, string? Visibility, string? Status, List<PlanDayDto>? Days);
