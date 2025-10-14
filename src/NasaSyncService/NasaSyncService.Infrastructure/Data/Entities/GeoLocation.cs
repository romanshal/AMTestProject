namespace NasaSyncService.Infrastructure.Data.Entities
{
    public class GeoLocation
    {
        public Guid LocationId { get; set; }
        public required string MeteoriteId { get; set; }
        public required string Type { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }


        public virtual Meteorite Meteorite { get; set; } = default!;
    }
}
