using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.OpenApi.Models;
using NasaSyncService.API.Configurations;
using NasaSyncService.API.Extensions;
using NasaSyncService.API.Middlewares;
using NasaSyncService.Application;
using NasaSyncService.Infrastructure;
using NasaSyncService.Infrastructure.Data.Contexts;
using Polly;

var builder = WebApplication.CreateBuilder(args);

var retryPolicy = builder.Configuration.GetRequiredSection("RetryPolicy").Get<RetryPolicy>() ?? throw new InvalidOperationException("Settings for RetryPolicy not found");

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

builder.Services
    .AddApiLogging(builder.Configuration, "NasaSyncService.API", builder.Environment.EnvironmentName)
    .AddApiMetrics(builder.Configuration, "NasaSyncService.API", "0.1.0", builder.Environment.EnvironmentName);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
        .WithOrigins("http://localhost:53858")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services
    .AddHttpClient("HttpCLient")
    .AddResilienceHandler("CustomPolly", builder =>
    {
        builder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = retryPolicy.Attempts,
            Delay = TimeSpan.FromSeconds(retryPolicy.DelaySeconds),
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true
        });

        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = retryPolicy.FailureRatio,
            MinimumThroughput = 10,
            SamplingDuration = TimeSpan.FromSeconds(retryPolicy.SamplingDurationSeconds),
            BreakDuration = TimeSpan.FromSeconds(retryPolicy.BreakDurationSeconds)
        });
    });

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NasaSyncService.API", Version = "v1" });
    options.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MigrateDatabase<NasaDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<DataSeedMaker>>();
    DataSeedMaker
        .SeedAsync(context, logger!)
        .Wait();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NasaSyncService.API v1");
        options.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowSpecificOrigin");

app.UseExeptionWrappingMiddleware();

app.MapControllers();

app.MapPrometheusScrapingEndpoint();

app.UseHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
