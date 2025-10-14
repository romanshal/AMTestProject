using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NasaSyncService.Application.Interfaces.Cache;
using NasaSyncService.Application.Interfaces.Repositories;
using NasaSyncService.Infrastructure.BackgroundServices;
using NasaSyncService.Infrastructure.Cache;
using NasaSyncService.Infrastructure.Data.Contexts;
using NasaSyncService.Infrastructure.Hash;
using NasaSyncService.Infrastructure.Repositories;
using NasaSyncService.Infrastructure.Settings;
using StackExchange.Redis;
using System.Reflection;

namespace NasaSyncService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnectionString = configuration.GetConnectionString("NasaDbConnection") ?? throw new InvalidOperationException("Connection string 'NasaDbConnection' not found");
            var redisSettings = configuration.GetSection("Redis").Get<RedisSettings>() ?? throw new InvalidOperationException("Connection settings for Redis not found");
            services.Configure<BackgroundServiceSettings>(configuration.GetSection("BackgroundService"));

            services.Configure<RedisSettings>(configuration.GetSection("Redis"));

            services.AddDbContext<NasaDbContext>(options =>
            {
                options.UseNpgsql(dbConnectionString);
            });

            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            services.AddHostedService<NasaSyncWorker>();

            services.AddTransient<IMeteoriteRepository, MeteoriteRepository>();
            services.AddTransient<IRecclassRepository, RecclassRepository>();

            services.AddStackExchangeRedisCache(options =>
            {
                var connection = $"{redisSettings.Host}:{redisSettings.Port},password={redisSettings.Password}";
                options.Configuration = connection;
            });

            var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { $"{redisSettings.Host}:{redisSettings.Port}" },
                AbortOnConnectFail = false,
                Ssl = false,
                Password = redisSettings.Password,
                TieBreaker = "",
                AllowAdmin = true,
                CommandMap = CommandMap.Create(
                [
                    "INFO", "CONFIG", "CLUSTER", "PING", "ECHO", "CLIENT"
                ], available: false)
            });
            services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);

            services.AddScoped<ICacheService, RedisCacheService>();

            services.AddSingleton<IHasher, Hasher>();

            services
                .AddHealthChecks()
                .AddNpgSql(dbConnectionString)
                .AddRedis($"{redisSettings.Host}:{redisSettings.Port}");

            return services;
        }
    }
}
