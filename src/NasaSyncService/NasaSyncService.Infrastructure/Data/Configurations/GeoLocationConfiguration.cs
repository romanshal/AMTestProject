using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasaSyncService.Infrastructure.Data.Entities;

namespace NasaSyncService.Infrastructure.Data.Configurations
{
    internal class GeoLocationConfiguration : IEntityTypeConfiguration<GeoLocation>
    {
        public void Configure(EntityTypeBuilder<GeoLocation> builder)
        {
            builder
                .ToTable("geolocations")
                .HasKey(k => k.LocationId);

            builder.Property(p => p.LocationId).ValueGeneratedOnAdd();

            builder.Property(p => p.Type).HasMaxLength(100).IsRequired();
        }
    }
}
