namespace NasaSyncService.Infrastructure.Data.Entities
{
    public class Recclass
    {
        public Guid ClassId { get; set; }
        public required string RecclassName { get; set; }

        public virtual ICollection<Meteorite> Meteorites { get; set; } = [];
    }
}
