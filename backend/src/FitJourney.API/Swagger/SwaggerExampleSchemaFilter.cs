using System.Text.Json.Nodes;
using FitJourney.Application.DTOs;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FitJourney.API.Swagger;

public class SwaggerExampleSchemaFilter : ISchemaFilter
{
    private static readonly IReadOnlyDictionary<Type, string> Examples = new Dictionary<Type, string>
    {
        [typeof(RegisterRequest)] = """
            { "name": "Edina Maljevac", "email": "edina@example.com", "password": "password123", "role": "user" }
            """,
        [typeof(LoginRequest)] = """
            { "email": "user1@fit.io", "password": "password123" }
            """,
        [typeof(RefreshRequest)] = """
            { "refreshToken": "9f8c1e2a-4b6d-4c2e-9a1f-2b3c4d5e6f70" }
            """,
        [typeof(AuthResponse)] = """
            {
              "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2YTI1...",
              "refreshToken": "9f8c1e2a-4b6d-4c2e-9a1f-2b3c4d5e6f70",
              "user": {
                "id": "6a269dd8872d49ac77d48cf1",
                "name": "Edina Maljevac",
                "email": "edina@example.com",
                "role": "user",
                "avatarUrl": null,
                "profile": { "gender": "female", "experience": "beginner", "goal": "muscle_gain" },
                "activePlanId": null
              }
            }
            """,
        [typeof(CreateSessionRequest)] = """
            {
              "planId": null,
              "startedAt": "2026-06-08T18:00:00Z",
              "endedAt": "2026-06-08T18:50:00Z",
              "notes": "Felt strong",
              "feeling": 4,
              "exercises": [
                {
                  "exerciseId": "6a25b0d6612ed3aa48e261c0",
                  "nameSnapshot": "Back Squat",
                  "type": "weighted",
                  "sets": [
                    { "setNumber": 1, "reps": 12, "weightKg": 50, "rpe": 8, "completed": true },
                    { "setNumber": 2, "reps": 10, "weightKg": 55, "rpe": 9, "completed": true }
                  ]
                }
              ]
            }
            """,
        [typeof(SessionDto)] = """
            {
              "id": "6a25b275f471b8ceac32ffe3",
              "userId": "6a269dd8872d49ac77d48cf1",
              "planId": null,
              "startedAt": "2026-06-08T18:00:00Z",
              "endedAt": "2026-06-08T18:50:00Z",
              "notes": "Felt strong",
              "feeling": 4,
              "totalVolumeKg": 1150,
              "completedSets": 2,
              "durationSec": 3000,
              "createdAt": "2026-06-08T18:50:05Z",
              "exercises": [
                {
                  "exerciseId": "6a25b0d6612ed3aa48e261c0",
                  "nameSnapshot": "Back Squat",
                  "type": "weighted",
                  "sets": [
                    { "setNumber": 1, "reps": 12, "weightKg": 50, "rpe": 8, "completed": true },
                    { "setNumber": 2, "reps": 10, "weightKg": 55, "rpe": 9, "completed": true }
                  ]
                }
              ]
            }
            """,
    };

    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is OpenApiSchema concrete && Examples.TryGetValue(context.Type, out var json))
            concrete.Example = JsonNode.Parse(json);
    }
}
