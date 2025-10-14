namespace NasaSyncService.Infrastructure.Settings
{
    internal class RedisSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public int ExpirationTimeSeconds { get; set; }
    }
}
