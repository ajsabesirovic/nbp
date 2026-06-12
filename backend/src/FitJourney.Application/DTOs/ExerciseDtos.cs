namespace FitJourney.Application.DTOs;

public record ExerciseDto(string Id, string Name, string Type, List<string> PrimaryMuscles, List<string> SecondaryMuscles, string? Category, string? Equipment, int? Difficulty, string? Description, string? Instructions, string? ImageUrl, string? VideoUrl, bool IsCustom, string? CreatedBy);
public record CreateExerciseRequest(string Name, string Type, List<string> PrimaryMuscles, List<string>? SecondaryMuscles, string? Category, string? Equipment, int? Difficulty, string? Description, string? Instructions, string? ImageUrl, string? VideoUrl);
public record UpdateExerciseRequest(string? Name, string? Type, List<string>? PrimaryMuscles, List<string>? SecondaryMuscles, string? Category, string? Equipment, int? Difficulty, string? Description, string? Instructions, string? ImageUrl, string? VideoUrl);
