using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasaSyncService.Infrastructure.Data.Entities;

namespace NasaSyncService.Infrastructure.Data.Configurations
{
    internal class MeteoriteConfiguration : IEntityTypeConfiguration<Meteorite>
    {
        public void Configure(EntityTypeBuilder<Meteorite> builder)
        {
            builder
                .ToTable("meteorites")
                .HasKey(k => k.MetioriteId);

            builder
                .Property(p => p.MetioriteId).ValueGeneratedNever();

            builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Nametype).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Fall).HasMaxLength(50).IsRequired();
            builder.Property(p => p.RecordHash).HasMaxLength(200).IsRequired();

            builder
                .HasMany(m => m.GeoLocations)
                .WithOne(o => o.Meteorite)
                .HasForeignKey(k => k.MeteoriteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(o => o.Recclass)
                .WithMany(m => m.Meteorites)
                .HasForeignKey(k => k.RecclassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.YearUtc)
                .HasDatabaseName("IX_Meteorites_YearUtc");

            builder.HasIndex(p => p.RecclassId)
                   .HasDatabaseName("IX_Meteorites_RecclassId");

            builder.HasIndex(p => p.Name)
                   .HasDatabaseName("IX_Meteorites_Name");

            builder.HasIndex(p => new { p.YearUtc, p.MassGram })
                   .HasDatabaseName("IX_Meteorites_YearUtc_MassGram");
        }
    }
}
