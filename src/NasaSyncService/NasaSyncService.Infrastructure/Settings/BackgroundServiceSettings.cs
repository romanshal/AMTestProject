namespace NasaSyncService.Infrastructure.Settings
{
    internal class BackgroundServiceSettings
    {
        public string Url { get; set; } = string.Empty;
        public int IntervalHours { get; set; }
    }
}
