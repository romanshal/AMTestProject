namespace NasaSyncService.Infrastructure.Data.Entities
{
    public class Snapshot
    {
        public Guid Id { get; set; }
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset FinishedAt { get; set; }
        public string SnapshotHash { get; set; } = default!;
        public string SourceUrl { get; set; } = default!;
        public int FetchedCount { get; set; }
        public int InsertedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SoftDeletedCount { get; set; }
        public bool SkippedSameHash { get; set; }
        public bool Status { get; set; }
        public string? Error { get; set; }
    }
}
