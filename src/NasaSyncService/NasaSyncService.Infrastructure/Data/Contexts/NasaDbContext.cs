using Microsoft.EntityFrameworkCore;
using NasaSyncService.Infrastructure.Data.Entities;
using System.Reflection;

namespace NasaSyncService.Infrastructure.Data.Contexts
{
    public class NasaDbContext(
        DbContextOptions<NasaDbContext> options) : DbContext(options)
    {
        public DbSet<Meteorite> Meteorites { get; set; }
        public DbSet<Recclass> Recclasses { get; set; }
        public DbSet<GeoLocation> GeoLocations { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(NasaDbContext))!);
        }
    }
}
