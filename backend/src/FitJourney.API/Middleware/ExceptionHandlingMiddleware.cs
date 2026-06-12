using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using FitJourney.Application.Common;
namespace FitJourney.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title, detail, errors) = exception switch
        {
            ValidationException ve => (422, "Validation Error", "One or more validation errors occurred.",
                ve.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToArray()),
            ForbiddenException => (403, "Forbidden", exception.Message, Array.Empty<string>()),
            UnauthorizedAccessException => (401, "Unauthorized", exception.Message, Array.Empty<string>()),
            KeyNotFoundException => (404, "Not Found", exception.Message, Array.Empty<string>()),
            InvalidOperationException => (409, "Conflict", exception.Message, Array.Empty<string>()),
            _ => (500, "Internal Server Error", "An unexpected error occurred.", Array.Empty<string>())
        };

        context.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path.Value,
        };
        if (errors.Length > 0)
            problem.Extensions["errors"] = errors;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        });
        await context.Response.WriteAsync(json);
    }
}
