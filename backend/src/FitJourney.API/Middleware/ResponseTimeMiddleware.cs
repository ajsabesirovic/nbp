using System.Diagnostics;
using System.Security.Claims;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;
namespace FitJourney.API.Middleware;

public class ResponseTimeMiddleware(RequestDelegate next, IConfiguration config)
{
    private readonly long _slowThresholdMs = config.GetValue<long?>("RequestLogging:SlowThresholdMs") ?? 500L;

    public async Task InvokeAsync(HttpContext context, IServiceProvider services)
    {
        var sw = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;

        context.Response.OnStarting(() =>
        {
            sw.Stop();
            context.Response.Headers["X-Response-Time"] = $"{sw.ElapsedMilliseconds}ms";
            context.Response.Headers["X-Request-Id"] = requestId;
            return Task.CompletedTask;
        });

        try
        {
            await next(context);
        }
        finally
        {
            if (sw.IsRunning) sw.Stop();

            var durationMs = sw.ElapsedMilliseconds;
            var slow = durationMs >= _slowThresholdMs;
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? context.User?.FindFirst("sub")?.Value;

            var entry = new RequestLog
            {
                Method = context.Request.Method,
                Path = context.Request.Path.Value ?? string.Empty,
                StatusCode = context.Response.StatusCode,
                DurationMs = durationMs,
                UserId = userId,
                RequestId = requestId,
                SlowRequest = slow,
                Timestamp = DateTime.UtcNow,
            };

            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = services.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IRequestLogRepository>();
                    await repo.InsertAsync(entry);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to persist request log for {RequestId}", requestId);
                }
            });

            if (slow)
                Log.Warning("Slow request {Method} {Path} took {DurationMs}ms (threshold {Threshold}ms)",
                    entry.Method, entry.Path, durationMs, _slowThresholdMs);
        }
    }
}
