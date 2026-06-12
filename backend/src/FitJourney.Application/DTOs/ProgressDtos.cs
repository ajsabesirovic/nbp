namespace FitJourney.Application.DTOs;

public record StreakDto(int CurrentStreak, int LongestStreak, int TotalWorkoutDays, DateTime? LastWorkoutDate);
public record WeeklyVolumeDto(int Year, int Week, double TotalVolumeKg, int SessionCount, int TotalSets);
public record MuscleBalanceDto(string Muscle, int Sets, double VolumeKg);
public record ProgressionDto(DateTime Date, double MaxWeightKg, double OneRepMax, int TotalSets, double TotalVolumeKg);
public record PersonalRecordDto(string Id, string ExerciseId, string? ExerciseName, string Type, double WeightKg, int Reps, double OneRepMax, DateTime AchievedAt);
