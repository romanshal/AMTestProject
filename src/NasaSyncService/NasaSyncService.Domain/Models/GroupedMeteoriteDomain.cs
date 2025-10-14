namespace NasaSyncService.Domain.Models
{
    public class GroupedMeteoriteDomain
    {
        public int? Year { get; set; }
        public int Count { get; set; }
        public decimal TotalMass { get; set; }
    }
}
