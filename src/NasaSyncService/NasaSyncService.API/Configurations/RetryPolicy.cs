namespace NasaSyncService.API.Configurations
{
    public class RetryPolicy
    {
        public int Attempts { get; set; }
        public int DelaySeconds { get; set; }
        public double FailureRatio { get; set; }
        public int Throughput { get; set; }
        public int SamplingDurationSeconds { get; set; }
        public int BreakDurationSeconds { get; set; }
    }
}
