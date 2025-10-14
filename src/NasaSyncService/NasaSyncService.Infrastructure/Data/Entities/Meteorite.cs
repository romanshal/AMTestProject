namespace NasaSyncService.Infrastructure.Data.Entities
{
    public class Meteorite
    {
        public required string MetioriteId { get; set; }
        public required string Name { get; set; }
        public required string Nametype { get; set; }
        public Guid RecclassId { get; set; }
        public decimal? MassGram { get; set; }
        public required string Fall { get; set; }
        public DateTimeOffset? YearUtc { get; set; }

        public double? Reclat { get; set; }
        public double? Reclong { get; set; }

        public string? Extra { get; set; } // JSON with unknown properties

        public string RecordHash { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        public virtual Recclass Recclass { get; set; } = default!;
        public virtual ICollection<GeoLocation> GeoLocations { get; set; } = [];
    }
}
