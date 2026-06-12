using System.Reflection;
using System.Text;
using Asp.Versioning;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using FitJourney.Application.Behaviors;
using FitJourney.Application.Validators;
using FitJourney.Infrastructure;
using FitJourney.Infrastructure.Services;
using FitJourney.Infrastructure.Settings;
using FitJourney.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://+:{port}");

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/fitjourney-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(FitJourney.Application.Features.Auth.Commands.RegisterCommand).Assembly));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(FitJourney.Application.Mapping.MappingProfile).Assembly);
    cfg.AddMaps(typeof(FitJourney.Infrastructure.Mapping.InfrastructureMappingProfile).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(RegisterRequestValidator).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

const string DevDefaultJwtSecret = "fitjourney-super-secret-key-change-in-production-must-be-32-chars";
if (string.IsNullOrWhiteSpace(jwtSettings.Secret) || jwtSettings.Secret.Length < 32)
    throw new InvalidOperationException(
        "Jwt:Secret is missing or shorter than 32 characters. Set it via the Jwt__Secret " +
        "environment variable (or appsettings.Development.json for local development).");
if (!builder.Environment.IsDevelopment() && jwtSettings.Secret == DevDefaultJwtSecret)
    throw new InvalidOperationException(
        "The development JWT secret must not be used outside Development. " +
        "Provide a unique Jwt__Secret environment variable.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AdminOnly", p => p.RequireRole("admin"));

    opts.AddPolicy("TrainerOnly", p => p.RequireRole("trainer"));
});

builder.Services.AddApiVersioning(opts =>
{
    opts.DefaultApiVersion = new ApiVersion(1);
    opts.AssumeDefaultVersionWhenUnspecified = true;
    opts.ReportApiVersions = true;
    opts.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(opts =>
{
    opts.GroupNameFormat = "'v'VVV";
    opts.SubstituteApiVersionInUrl = true;
});

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    var resolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver();
    resolver.Modifiers.Add(FitJourney.API.Json.MongoIdAliasModifier.AddIdAlias);
    opts.JsonSerializerOptions.TypeInfoResolver = resolver;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FitJourney API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT token"
    });
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            []
        }
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
    c.SchemaFilter<FitJourney.API.Swagger.SwaggerExampleSchemaFilter>();
});

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
var allowedOrigins = new[] { "http://localhost:5173", "http://localhost:5174" }
    .Concat(corsOrigins)
    .ToArray();
builder.Services.AddCors(opts => opts.AddDefaultPolicy(p =>
    p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.RunAsync();
    return;
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ResponseTimeMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads"));
app.UseStaticFiles();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow }));

app.Run();
