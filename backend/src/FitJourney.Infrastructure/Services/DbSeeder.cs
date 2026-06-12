using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;

namespace FitJourney.Infrastructure.Services;

public class DbSeeder(
    MongoDbContext db,
    IUserRepository users,
    IExerciseRepository exercises,
    IWorkoutPlanRepository plans,
    ITrainerProfileRepository trainerProfiles,
    IPlanAssignmentRepository assignments,
    IWorkoutSessionRepository sessions,
    IBodyMeasurementRepository measurements,
    IMessageRepository messages,
    INotificationRepository notifications,
    IPersonalRecordRepository personalRecords,
    ILogger<DbSeeder> logger)
{

    private readonly Random _rng = new(42);
    private readonly DateTime _today = DateTime.UtcNow.Date;

    public async Task RunAsync()
    {
        logger.LogInformation("Clearing collections...");
        await Task.WhenAll(
            db.Users.DeleteManyAsync(Builders<UserDocument>.Filter.Empty),
            db.Exercises.DeleteManyAsync(Builders<ExerciseDocument>.Filter.Empty),
            db.Plans.DeleteManyAsync(Builders<WorkoutPlanDocument>.Filter.Empty),
            db.Sessions.DeleteManyAsync(Builders<WorkoutSessionDocument>.Filter.Empty),
            db.PersonalRecords.DeleteManyAsync(Builders<PersonalRecordDocument>.Filter.Empty),
            db.RefreshTokens.DeleteManyAsync(Builders<RefreshTokenDocument>.Filter.Empty),
            db.TrainerProfiles.DeleteManyAsync(Builders<TrainerProfileDocument>.Filter.Empty),
            db.PlanAssignments.DeleteManyAsync(Builders<PlanAssignmentDocument>.Filter.Empty),
            db.BodyMeasurements.DeleteManyAsync(Builders<BodyMeasurementDocument>.Filter.Empty),
            db.Messages.DeleteManyAsync(Builders<MessageDocument>.Filter.Empty),
            db.Notifications.DeleteManyAsync(Builders<NotificationDocument>.Filter.Empty),
            db.ProgressPhotos.DeleteManyAsync(Builders<ProgressPhotoDocument>.Filter.Empty));

        logger.LogInformation("Inserting users...");
        var admin = await CreateUser("admin@fit.io", "Admin Adminović", UserRole.admin);
        var trainer = await CreateUser("besirovicajsa@gmail.com", "Ajsa Beširović", UserRole.trainer);
        var edina = await CreateUser("maljevacedina@gmail.com", "Edina Maljevac", UserRole.user, new UserProfile
        {
            Gender = "female",
            DateOfBirth = new DateTime(1999, 4, 12),
            HeightCm = 168,
            CurrentWeightKg = 74,
            TargetWeightKg = 66,
            Experience = "beginner",
            Goal = "muscle_gain",
        });
        var user1 = await CreateUser("user1@fit.io", "Uros User", UserRole.user, Profile("male", 1995, 182, 88, 82, "intermediate", "weight_loss"));
        var user2 = await CreateUser("user2@fit.io", "Una User", UserRole.user, Profile("female", 1997, 165, 60, 62, "beginner", "general_fitness"));
        var user3 = await CreateUser("user3@fit.io", "Uma User", UserRole.user, Profile("female", 2000, 171, 69, 65, "advanced", "endurance"));

        logger.LogInformation("Inserting exercises...");
        var byName = new Dictionary<string, Exercise>();
        foreach (var seed in ExerciseSeed.All)
        {
            var created = await exercises.CreateAsync(new Exercise
            {
                Name = seed.Name,
                Type = seed.Type,
                PrimaryMuscles = seed.Primary,
                SecondaryMuscles = seed.Secondary,
                Category = DeriveCategory(seed.Type),
                Equipment = DeriveEquipment(seed.Name, seed.Type),
                Difficulty = DeriveDifficulty(seed.Name, seed.Type),
                Description = $"{seed.Name} — targets {string.Join(", ", seed.Primary)}.",
                Instructions = "Maintain controlled form through the full range of motion; brace the core.",
                IsCustom = false,
                CreatedAt = DateTime.UtcNow,
            });
            byName[seed.Name] = created;
        }

        logger.LogInformation("Inserting trainer profile + clients...");
        await trainerProfiles.UpsertAsync(new TrainerProfile
        {
            UserId = trainer.Id,
            Certifications = ["NASM-CPT", "Precision Nutrition L1", "FMS Level 2"],
            Specialization = "Strength & hypertrophy for beginners and intermediates",
            PricePerPlan = 59.99,
            Bio = "Coaching since 2017. I focus on solid technique, progressive overload, and habits that last.",
            ClientIds = [edina.Id, user1.Id, user2.Id, user3.Id],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });

        logger.LogInformation("Inserting workout plans...");
        var beginnerPlan = await plans.CreateAsync(new WorkoutPlan
        {
            AuthorId = trainer.Id,
            AuthorName = trainer.Name,
            Name = "Beginner Full Body 3x/week",
            Description = "A simple full-body plan for new lifters.",
            DurationWeeks = 8,
            Level = PlanLevel.beginner,
            Goal = PlanGoal.general_fitness,
            DaysPerWeek = 3,
            Visibility = Visibility.@public,
            Status = "published",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Days =
            [
                Day(1, "Day A", PlanRow(byName["Back Squat"], 3, "5-8", 120), PlanRow(byName["Bench Press"], 3, "5-8", 120), PlanRow(byName["Barbell Row"], 3, "8-10", 90), PlanRow(byName["Plank"], 3, "45s", 60)),
                Day(2, "Day B", PlanRow(byName["Romanian Deadlift"], 3, "8", 120), PlanRow(byName["Overhead Press"], 3, "6-8", 120), PlanRow(byName["Lat Pulldown"], 3, "10-12", 90), PlanRow(byName["Bicep Curl"], 3, "12", 60)),
                Day(3, "Day C", PlanRow(byName["Front Squat"], 3, "6-8", 120), PlanRow(byName["Incline Dumbbell Press"], 3, "8-10", 90), PlanRow(byName["Pull-up"], 3, "AMRAP", 120), PlanRow(byName["Hip Thrust"], 3, "10", 90)),
            ],
        });

        var intermediatePlan = await plans.CreateAsync(new WorkoutPlan
        {
            AuthorId = trainer.Id,
            AuthorName = trainer.Name,
            Name = "Intermediate Upper/Lower 4x/week",
            Description = "Four-day upper/lower split for hypertrophy.",
            DurationWeeks = 10,
            Level = PlanLevel.intermediate,
            Goal = PlanGoal.muscle_gain,
            DaysPerWeek = 4,
            Visibility = Visibility.@public,
            Status = "published",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Days =
            [
                Day(1, "Upper A", PlanRow(byName["Bench Press"], 4, "6-8", 120), PlanRow(byName["Barbell Row"], 4, "6-8", 120), PlanRow(byName["Overhead Press"], 3, "8-10", 90), PlanRow(byName["Tricep Pushdown"], 3, "12-15", 60)),
                Day(2, "Lower A", PlanRow(byName["Back Squat"], 4, "5-8", 150), PlanRow(byName["Romanian Deadlift"], 3, "8-10", 120), PlanRow(byName["Leg Press"], 3, "12", 90), PlanRow(byName["Wall Sit"], 3, "45s", 60)),
                Day(3, "Upper B", PlanRow(byName["Incline Dumbbell Press"], 4, "8-10", 90), PlanRow(byName["Lat Pulldown"], 4, "10-12", 90), PlanRow(byName["Seated Cable Row"], 3, "10-12", 90), PlanRow(byName["Bicep Curl"], 3, "12-15", 60)),
                Day(4, "Lower B", PlanRow(byName["Conventional Deadlift"], 4, "4-6", 180), PlanRow(byName["Walking Lunge"], 3, "10", 90), PlanRow(byName["Hip Thrust"], 3, "10-12", 90), PlanRow(byName["Hollow Hold"], 3, "30s", 45)),
            ],
        });

        await plans.CreateAsync(new WorkoutPlan
        {
            AuthorId = edina.Id,
            AuthorName = edina.Name,
            Name = "My home dumbbell routine",
            Description = "Quick private routine for busy days.",
            DurationWeeks = 4,
            Level = PlanLevel.beginner,
            Goal = PlanGoal.general_fitness,
            DaysPerWeek = 2,
            Visibility = Visibility.@private,
            Status = "published",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Days = [Day(1, "Full body", PlanRow(byName["Push-up"], 3, "12", 60), PlanRow(byName["Bodyweight Squat"], 3, "15", 60), PlanRow(byName["Plank"], 3, "45s", 45))],
        });

        logger.LogInformation("Assigning plans to clients...");
        await assignments.AssignAsync(beginnerPlan.Id, edina.Id, trainer.Id);
        await assignments.AssignAsync(beginnerPlan.Id, user1.Id, trainer.Id);
        await assignments.AssignAsync(beginnerPlan.Id, user2.Id, trainer.Id);
        await assignments.AssignAsync(intermediatePlan.Id, user3.Id, trainer.Id);

        edina.ActivePlanId = beginnerPlan.Id;
        await users.UpdateAsync(edina);

        logger.LogInformation("Generating workout sessions...");
        await GenerateSessions(edina, beginnerPlan, byName, weeks: 8, perWeek: 3, recentStreakDays: 6, computePrs: true);
        await GenerateSessions(user1, beginnerPlan, byName, weeks: 5, perWeek: 3, recentStreakDays: 0, computePrs: false);
        await GenerateSessions(user2, beginnerPlan, byName, weeks: 4, perWeek: 2, recentStreakDays: 0, computePrs: false);
        await GenerateSessions(user3, intermediatePlan, byName, weeks: 3, perWeek: 4, recentStreakDays: 0, computePrs: false);

        logger.LogInformation("Generating body measurements...");
        await GenerateMeasurements(edina, weeks: 8, startWeight: 78, endWeight: 74);
        await GenerateMeasurements(user1, weeks: 5, startWeight: 92, endWeight: 88);

        logger.LogInformation("Seeding messages + notifications...");
        await SeedConversation(edina, trainer);
        await notifications.CreateAsync(new Notification
        {
            UserId = edina.Id, Type = "welcome", Title = "Welcome to FitJourney!",
            Body = "Browse the exercise library, follow a plan, and log your first workout.",
            Link = "/plans", CreatedAt = _today.AddDays(-56),
        });

        logger.LogInformation(
            "Seed complete. Trainer: besirovicajsa@gmail.com · User: maljevacedina@gmail.com · Admin: admin@fit.io · password: password123");
    }

    private async Task GenerateSessions(User user, WorkoutPlan plan, Dictionary<string, Exercise> byName,
        int weeks, int perWeek, int recentStreakDays, bool computePrs)
    {
        var start = _today.AddDays(-7 * weeks);

        int[] workoutDows = perWeek switch
        {
            >= 4 => [1, 2, 4, 5],
            3 => [1, 3, 5],
            _ => [2, 5],
        };
        var workoutDays = new SortedSet<DateTime>();
        for (var d = start; d <= _today; d = d.AddDays(1))
            if (workoutDows.Contains((int)d.DayOfWeek)) workoutDays.Add(d);

        for (int i = 0; i < recentStreakDays; i++) workoutDays.Add(_today.AddDays(-i));

        var bestByExercise = new Dictionary<string, (Exercise Ex, double W, int Reps, double Orm)>();
        int dayCursor = 0;

        foreach (var date in workoutDays)
        {
            var planDay = plan.Days[dayCursor % plan.Days.Count];
            dayCursor++;
            int weekIndex = (date - start).Days / 7;

            var performed = new List<PerformedExercise>();
            double volume = 0;
            int completedSets = 0;

            foreach (var row in planDay.Exercises)
            {
                if (!byName.TryGetValue(row.NameSnapshot ?? "", out var ex)) continue;
                var (sets, exVolume) = BuildSets(ex, weekIndex);
                volume += exVolume;
                completedSets += sets.Count;
                performed.Add(new PerformedExercise
                {
                    ExerciseId = ex.Id, NameSnapshot = ex.Name, Type = ex.Type.ToString(), Sets = sets,
                });

                if (computePrs && ex.Type == ExerciseType.weighted)
                {
                    foreach (var s in sets.Where(s => s.WeightKg.HasValue && s.Reps.HasValue))
                    {
                        double orm = s.WeightKg!.Value * (1 + s.Reps!.Value / 30.0);
                        if (!bestByExercise.TryGetValue(ex.Id, out var b) || orm > b.Orm)
                            bestByExercise[ex.Id] = (ex, s.WeightKg.Value, (int)s.Reps.Value, orm);
                    }
                }
            }

            var startedAt = date.AddHours(18);
            int durationSec = 60 * (40 + _rng.Next(0, 25));
            await sessions.CreateAsync(new WorkoutSession
            {
                UserId = user.Id,
                PlanId = plan.Id,
                PlanDayNumber = planDay.DayNumber,
                StartedAt = startedAt,
                EndedAt = startedAt.AddSeconds(durationSec),
                Exercises = performed,
                Notes = _rng.Next(0, 3) == 0 ? "Felt strong today." : "",
                Feeling = 3 + _rng.Next(0, 3),
                TotalVolumeKg = Math.Round(volume),
                CompletedSets = completedSets,
                DurationSec = durationSec,
                CreatedAt = startedAt,
            });
        }

        if (computePrs)
            foreach (var (_, b) in bestByExercise)
                await SeedPrs(user, b.Ex, b.W, b.Reps, b.Orm);
    }

    private (List<SessionSet> Sets, double Volume) BuildSets(Exercise ex, int weekIndex)
    {
        var sets = new List<SessionSet>();
        double volume = 0;
        switch (ex.Type)
        {
            case ExerciseType.weighted:
                double weight = BaseWeight(ex.Name) + weekIndex * 2.5;
                int[] reps = [10, 8, 6];
                for (int i = 0; i < reps.Length; i++)
                {
                    sets.Add(new SessionSet { SetNumber = i + 1, Reps = reps[i], WeightKg = weight, Rpe = 7 + i, Completed = true });
                    volume += weight * reps[i];
                }
                break;
            case ExerciseType.bodyweight:
                int[] bw = [12, 10, 8];
                for (int i = 0; i < bw.Length; i++)
                    sets.Add(new SessionSet { SetNumber = i + 1, Reps = bw[i], Rpe = 7 + i, Completed = true });
                break;
            case ExerciseType.isometric:
                for (int i = 0; i < 3; i++)
                    sets.Add(new SessionSet { SetNumber = i + 1, DurationSec = 45 - i * 5, Completed = true });
                break;
            case ExerciseType.endurance:
                sets.Add(new SessionSet { SetNumber = 1, DurationSec = 1800, DistanceM = 5000, Completed = true });
                break;
            default:
                sets.Add(new SessionSet { SetNumber = 1, Reps = 10, Completed = true });
                break;
        }
        return (sets, volume);
    }

    private async Task SeedPrs(User user, Exercise ex, double weight, int reps, double orm)
    {
        var achieved = _today.AddDays(-_rng.Next(1, 10)).AddHours(18);
        await personalRecords.UpsertAsync(new PersonalRecord
        {
            UserId = user.Id, ExerciseId = ex.Id, ExerciseName = ex.Name, Type = "1rm",
            WeightKg = weight, Reps = reps, OneRepMax = Math.Round(orm, 1), AchievedAt = achieved, CreatedAt = achieved,
        });
        await personalRecords.UpsertAsync(new PersonalRecord
        {
            UserId = user.Id, ExerciseId = ex.Id, ExerciseName = ex.Name, Type = "5rm",
            WeightKg = weight, Reps = Math.Max(reps, 5), OneRepMax = Math.Round(orm, 1), AchievedAt = achieved, CreatedAt = achieved,
        });
    }

    private async Task GenerateMeasurements(User user, int weeks, double startWeight, double endWeight)
    {
        for (int w = 0; w <= weeks; w++)
        {
            double t = (double)w / weeks;
            double weight = Math.Round(startWeight + (endWeight - startWeight) * t, 1);
            var date = _today.AddDays(-7 * (weeks - w)).AddHours(7);
            await measurements.CreateAsync(new BodyMeasurement
            {
                UserId = user.Id, Date = date, WeightKg = weight,
                WaistCm = Math.Round(82 - 6 * t, 1), ChestCm = Math.Round(96 + 1 * t, 1),
                ArmCm = Math.Round(31 + 1.5 * t, 1), ThighCm = Math.Round(57 - 2 * t, 1),
                BodyFatPct = Math.Round(28 - 5 * t, 1), CreatedAt = date,
            });
        }
    }

    private async Task SeedConversation(User client, User trainer)
    {
        DateTime t0 = _today.AddDays(-10).AddHours(9);
        await messages.CreateAsync(Msg(client, trainer, "Hi! Should I increase the squat weight this week?", t0, read: true));
        await messages.CreateAsync(Msg(trainer, client, "Yes — add 2.5 kg if last week felt like RPE 8 or below. Keep the depth consistent.", t0.AddHours(2), read: true));
        await messages.CreateAsync(Msg(client, trainer, "Got it, thank you! 🙏", t0.AddHours(3), read: true));
        await messages.CreateAsync(Msg(trainer, client, "Great progress on your last sessions, keep it up!", _today.AddDays(-1).AddHours(18), read: false));
    }

    private static Message Msg(User from, User to, string body, DateTime at, bool read) => new()
    {
        FromUserId = from.Id, ToUserId = to.Id, FromName = from.Name, ToName = to.Name,
        Body = body, CreatedAt = at, ReadAt = read ? at.AddMinutes(5) : null,
    };

    private static UserProfile Profile(string gender, int birthYear, double height, double cur, double target, string exp, string goal) => new()
    {
        Gender = gender, DateOfBirth = new DateTime(birthYear, 6, 1), HeightCm = height,
        CurrentWeightKg = cur, TargetWeightKg = target, Experience = exp, Goal = goal,
    };

    private async Task<User> CreateUser(string email, string name, UserRole role, UserProfile? profile = null) =>
        await users.CreateAsync(new User
        {
            Email = email.ToLowerInvariant(),
            Name = name,
            Role = role,
            Profile = profile,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123", 12),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });

    private static WorkoutPlanDay Day(int number, string name, params PlanExercise[] rows) =>
        new() { DayNumber = number, Name = name, Exercises = [.. rows] };

    private static PlanExercise PlanRow(Exercise ex, int sets, string reps, int rest) => new()
    {
        ExerciseId = ex.Id, NameSnapshot = ex.Name, Sets = sets, Reps = reps, RestSeconds = rest,
    };

    private static string DeriveCategory(ExerciseType t) => t switch
    {
        ExerciseType.endurance => "cardio",
        ExerciseType.isometric => "mobility",
        ExerciseType.flexibility => "flexibility",
        _ => "strength",
    };

    private static string DeriveEquipment(string name, ExerciseType t)
    {
        if (name.Contains("Dumbbell")) return "dumbbell";
        if (name.Contains("Cable") || name.Contains("Pulldown") || name.Contains("Pushdown")) return "cable";
        if (name.Contains("Leg Press") || name.Contains("Machine")) return "machine";
        if (t is ExerciseType.bodyweight or ExerciseType.isometric) return "bodyweight";
        if (t == ExerciseType.endurance) return "none";
        return "barbell";
    }

    private static int DeriveDifficulty(string name, ExerciseType t)
    {
        if (name.Contains("Deadlift") || name.Contains("Squat") || name.Contains("Pistol")) return 4;
        return t switch { ExerciseType.weighted => 3, _ => 2 };
    }

    private static double BaseWeight(string name) => name switch
    {
        "Conventional Deadlift" => 80,
        "Back Squat" => 60,
        "Romanian Deadlift" => 60,
        "Leg Press" => 100,
        "Hip Thrust" => 70,
        "Front Squat" => 45,
        "Bench Press" => 45,
        "Barbell Row" => 45,
        "Lat Pulldown" => 45,
        "Seated Cable Row" => 45,
        "Overhead Press" => 30,
        "Incline Dumbbell Press" => 22,
        "Walking Lunge" => 20,
        "Tricep Pushdown" => 20,
        "Bicep Curl" => 12,
        _ => 30,
    };
}
