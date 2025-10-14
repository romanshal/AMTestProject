using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NasaSyncService.Infrastructure.Data.Contexts;
using NasaSyncService.Infrastructure.Data.Entities;
using NasaSyncService.Infrastructure.Hash;
using NasaSyncService.Infrastructure.Settings;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NasaSyncService.Infrastructure.BackgroundServices
{
    internal class NasaSyncWorker(
        IServiceProvider sp,
        ILogger<NasaSyncWorker> logger,
        IOptions<BackgroundServiceSettings> options) : BackgroundService
    {
        private readonly ILogger<NasaSyncWorker> _logger = logger;
        private readonly BackgroundServiceSettings _settings = options.Value;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromHours(_settings.IntervalHours));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunSync(stoppingToken);
            }
        }

        private async Task RunSync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background service start.");

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NasaDbContext>();
            var http = scope.ServiceProvider.GetRequiredService<HttpClient>();
            var hasher = scope.ServiceProvider.GetRequiredService<IHasher>();

            var snapshot = new Snapshot
            {
                StartedAt = DateTimeOffset.UtcNow,
                SourceUrl = _settings.Url
            };

            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var lastEtag = await db.Snapshots
                    .AsNoTracking()
                    .OrderByDescending(s => s.StartedAt)
                    .Select(s => s.SnapshotHash)
                    .FirstOrDefaultAsync(cancellationToken);

                var request = new HttpRequestMessage(HttpMethod.Get, _settings.Url);
                if (!string.IsNullOrEmpty(lastEtag))
                    request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(lastEtag));

                var response = await http.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    snapshot.SkippedSameHash = true;
                    snapshot.Status = true;
                    snapshot.FinishedAt = DateTimeOffset.UtcNow;

                    db.Snapshots.Add(snapshot);
                    await db.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    return;
                }

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);

                snapshot.SnapshotHash = response.Headers.ETag?.Tag ?? hasher.ComputeHash(json);

                var items = JsonSerializer.Deserialize<List<JsonElement>>(json)!;
                snapshot.FetchedCount = items.Count;

                // Download all recclasses
                var recclasses = await db.Recclasses
                    .ToDictionaryAsync(r => r.RecclassName, r => r, cancellationToken);

                var ids = items.Select(i => i.GetProperty("id").GetString()!).ToList();
                var existing = await db.Meteorites
                    .Where(m => ids.Contains(m.MetioriteId))
                    .ToDictionaryAsync(m => m.MetioriteId, cancellationToken);

                foreach (var item in items)
                {
                    var id = item.GetProperty("id").GetString()!;
                    var recordHash = hasher.ComputeHash(item.GetRawText());

                    var recclassName = item.TryGetProperty("recclass", out var rc) ? rc.GetString()! : "Unknown";
                    var recclassId = EnsureRecclass(recclasses, db, recclassName);

                    if (!existing.TryGetValue(id, out var entity))
                    {
                        // INSERT
                        entity = MapMeteorite(item, recclassId, recordHash);
                        db.Meteorites.Add(entity);
                        snapshot.InsertedCount++;
                    }
                    else if (entity.RecordHash != recordHash)
                    {
                        // UPDATE
                        UpdateMeteorite(entity, item, recclassId, recordHash);
                        entity.UpdatedAt = DateTimeOffset.UtcNow;
                        snapshot.UpdatedCount++;
                    }
                }

                // Soft-delete
                var currentIds = ids.ToHashSet();
                var toDelete = await db.Meteorites
                    .Where(m => !currentIds.Contains(m.MetioriteId) && !m.IsDeleted)
                    .ToListAsync(cancellationToken);

                foreach (var m in toDelete)
                {
                    m.IsDeleted = true;
                    m.DeletedAt = DateTimeOffset.UtcNow;
                    snapshot.SoftDeletedCount++;
                }

                snapshot.Status = true;
                snapshot.FinishedAt = DateTimeOffset.UtcNow;
                db.Snapshots.Add(snapshot);

                await db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Background service end.");
            }
            catch (Exception ex)
            {
                snapshot.Status = false;
                snapshot.Error = ex.Message;
                _logger.LogError(ex, "Error in background service");
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private static Guid EnsureRecclass(
            Dictionary<string, Recclass> cache,
            NasaDbContext db,
            string recclassName)
        {
            if (string.IsNullOrWhiteSpace(recclassName))
                recclassName = "Unknown";

            if (!cache.TryGetValue(recclassName, out var recclass))
            {
                recclass = new Recclass
                {
                    ClassId = Guid.NewGuid(),
                    RecclassName = recclassName
                };
                db.Recclasses.Add(recclass);
                cache[recclassName] = recclass;
            }

            return recclass.ClassId;
        }

        private static Meteorite MapMeteorite(JsonElement item, Guid recclassId, string recordHash)
        {
            var meteorite = new Meteorite
            {
                MetioriteId = item.GetProperty("id").GetString()!,
                Name = item.GetProperty("name").GetString(),
                Nametype = item.GetProperty("nametype").GetString(),
                RecclassId = recclassId,
                MassGram = item.TryGetProperty("mass", out var mass) ? decimal.Parse(mass.GetString()!) : null,
                Fall = item.TryGetProperty("fall", out var fall) ? fall.GetString() : null,
                YearUtc = item.TryGetProperty("year", out var year) ? DateTimeOffset.Parse(year.GetString()!) : null,
                Reclat = item.TryGetProperty("reclat", out var lat) ? double.Parse(lat.GetString()!) : null,
                Reclong = item.TryGetProperty("reclong", out var lng) ? double.Parse(lng.GetString()!) : null,
                Extra = ExtractExtra(item),
                RecordHash = recordHash
            };

            meteorite.GeoLocations = ParseGeoLocatons(item, meteorite.MetioriteId);

            return meteorite;
        }

        private static void UpdateMeteorite(Meteorite entity, JsonElement item, Guid recclassId, string recordHash)
        {
            entity.Name = item.GetProperty("name").GetString();
            entity.Nametype = item.GetProperty("nametype").GetString();
            entity.MassGram = item.TryGetProperty("mass", out var mass) ? decimal.Parse(mass.GetString()!) : null;
            entity.Fall = item.TryGetProperty("fall", out var fall) ? fall.GetString() : null;
            entity.YearUtc = item.TryGetProperty("year", out var year) ? DateTimeOffset.Parse(year.GetString()!) : null;
            entity.Reclat = item.TryGetProperty("reclat", out var lat) ? double.Parse(lat.GetString()!) : null;
            entity.Reclong = item.TryGetProperty("reclong", out var lng) ? double.Parse(lng.GetString()!) : null;
            entity.Extra = ExtractExtra(item);
            entity.RecordHash = recordHash;

            entity.GeoLocations.Clear();
            entity.GeoLocations = ParseGeoLocatons(item, entity.MetioriteId);
        }

        private static List<GeoLocation> ParseGeoLocatons(JsonElement item, string meteoriteId)
        {
            var geoLocations = new List<GeoLocation>();

            if (item.TryGetProperty("geolocation", out var geo) && geo.ValueKind == JsonValueKind.Object)
            {
                var type = geo.TryGetProperty("type", out var typeProp) ? typeProp.GetString() ?? "Unknown" : "Unknown";

                if (geo.TryGetProperty("coordinates", out var coords) && coords.ValueKind == JsonValueKind.Array && coords.GetArrayLength() == 2)
                {
                    var geoLng = coords[0].GetDouble();
                    var geoLat = coords[1].GetDouble();

                    geoLocations.Add(new GeoLocation
                    {
                        MeteoriteId = meteoriteId,
                        Type = type,
                        Latitude = (decimal)geoLat,
                        Longitude = (decimal)geoLng
                    });
                }
            }

            return geoLocations;
        }

        private static string? ExtractExtra(JsonElement item)
        {
            var extra = new Dictionary<string, JsonElement>();

            foreach (var prop in item.EnumerateObject())
            {
                if (prop.Name.StartsWith(":@"))
                {
                    extra[prop.Name] = prop.Value;
                }
            }

            return extra.Count > 0
                ? JsonSerializer.Serialize(extra)
                : null;
        }
    }
}
