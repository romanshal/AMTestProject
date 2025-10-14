using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasaSyncService.Infrastructure.Data.Entities;

namespace NasaSyncService.Infrastructure.Data.Configurations
{
    public class SnapshotConfiguration : IEntityTypeConfiguration<Snapshot>
    {
        public void Configure(EntityTypeBuilder<Snapshot> builder)
        {
            builder
                .ToTable("snapshots")
                .HasKey(k => k.Id);

            builder
                .Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.SnapshotHash).HasMaxLength(2048).IsRequired();
            builder.Property(p => p.SourceUrl).HasMaxLength(2048).IsRequired();
            builder.Property(p => p.Error).HasMaxLength(4096);
        }
    }
}
