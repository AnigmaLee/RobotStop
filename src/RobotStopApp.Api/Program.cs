using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using RobotStopApp.Api.Auth;
using RobotStopApp.Api.Models;
using RobotStopApp.Api.Robot;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/robotstopapp-.log", rollingInterval: RollingInterval.Day));

var apiKey = Environment.GetEnvironmentVariable("ROBOTSTOPAPP_APIKEY")
             ?? builder.Configuration["ApiKey"];

builder.Services
    .AddAuthentication(ApiKeyOptions.DefaultScheme)
    .AddScheme<ApiKeyOptions, ApiKeyAuthenticationHandler>(
        ApiKeyOptions.DefaultScheme,
        opts => opts.ApiKey = apiKey);
builder.Services.AddAuthorization();

builder.Services.AddSingleton<IRobotController, StubRobotController>();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RobotStopApp API", Version = "v1" });
    var scheme = new OpenApiSecurityScheme
    {
        Name = ApiKeyOptions.HeaderName,
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "API key required for robot commands.",
        Reference = new OpenApiReference { Id = "ApiKey", Type = ReferenceType.SecurityScheme }
    };
    c.AddSecurityDefinition("ApiKey", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = Array.Empty<string>() });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    var feature = ctx.Features.Get<IExceptionHandlerFeature>();
    var logger = ctx.RequestServices.GetRequiredService<ILoggerFactory>()
        .CreateLogger("UnhandledException");
    logger.LogError(feature?.Error, "Unhandled exception");
    ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(new ErrorResponse(
        "InternalServerError",
        app.Environment.IsDevelopment() ? feature?.Error.Message : null));
}));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();

public partial class Program { }
