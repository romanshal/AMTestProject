using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasaSyncService.Infrastructure.Data.Entities;

namespace NasaSyncService.Infrastructure.Data.Configurations
{
    public class RecclassConfiguration : IEntityTypeConfiguration<Recclass>
    {
        public void Configure(EntityTypeBuilder<Recclass> builder)
        {
            builder
                .ToTable("recclasses")
                .HasKey(k => k.ClassId);

            builder.Property(p => p.ClassId).ValueGeneratedNever();

            builder.Property(p => p.RecclassName).HasMaxLength(100).IsRequired();
        }
    }
}
