using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NasaSyncService.Infrastructure.Data.Contexts;
using System.Reflection;
using Testcontainers.PostgreSql;

namespace NasaSyncService.Test.Units
{
    public class InfrastructureTestFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder().Build();

        public NasaDbContext GetDbContext()
        {
            var context =  new NasaDbContext(new DbContextOptionsBuilder<NasaDbContext>().UseNpgsql(dbContainer.GetConnectionString()).Options);
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE \"meteorites\" CASCADE;");
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE \"recclasses\" CASCADE;");

            return context;
        }

        public async Task InitializeAsync()
        {
            await dbContainer.StartAsync();

            var migrationAssembly = typeof(NasaDbContext)
                .GetTypeInfo()
                .Assembly
                .GetName()
                .Name;

            var options = new DbContextOptionsBuilder<NasaDbContext>()
                .UseNpgsql(dbContainer.GetConnectionString(), npqsqlOptions =>
                    npqsqlOptions.MigrationsAssembly(migrationAssembly))
                .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
                .Options;

            using var nasadDbContext = new NasaDbContext(options);

            await nasadDbContext.Database.MigrateAsync();
        }

        public async Task DisposeAsync() => await dbContainer.DisposeAsync();
    }
}
