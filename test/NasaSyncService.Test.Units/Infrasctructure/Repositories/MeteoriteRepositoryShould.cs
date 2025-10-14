using FluentAssertions;
using NasaSyncService.Infrastructure.Data.Entities;
using NasaSyncService.Infrastructure.Repositories;

namespace NasaSyncService.Test.Units.Infrasctructure.Repositories
{
    public class MeteoriteRepositoryShould(InfrastructureTestFixture fixture) : IClassFixture<InfrastructureTestFixture>
    {
        private readonly InfrastructureTestFixture _fixture = fixture;

        private static Meteorite CreateMeteorite(
            string id,
            string name,
            int? year,
            decimal mass,
            Guid recclassId)
        {
            return new Meteorite
            {
                MetioriteId = id,
                Name = name,
                RecclassId = recclassId,
                Nametype = "TestNameType",
                Fall = "Found",
                YearUtc = year.HasValue ? new DateTimeOffset(year.Value, 1, 1, 0, 0, 0, TimeSpan.Zero) : null,
                MassGram = mass,
                RecordHash = Guid.NewGuid().ToString()
            };
        }

        [Fact]
        public async Task ReturnEmpty_WhenNoMeteorites()
        {
            await using var dbContext = _fixture.GetDbContext();
            var sut = new MeteoriteRepository(dbContext);

            var result = await sut.GetGroupedAsync(null, null, null, null, "year");

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GroupByYear_AndReturnCountsAndMass()
        {
            await using var dbContext = _fixture.GetDbContext();

            var recclassId = Guid.NewGuid();
            dbContext.Recclasses.Add(new Recclass 
            { 
                ClassId = recclassId, 
                RecclassName = "test" 
            });
            dbContext.Meteorites.AddRange(
                CreateMeteorite("1", "A", 2000, 10, recclassId),
                CreateMeteorite("2", "B", 2000, 20, recclassId),
                CreateMeteorite("3", "C", 2001, 30, recclassId)
            );

            await dbContext.SaveChangesAsync();

            var sut = new MeteoriteRepository(dbContext);

            var result = (await sut.GetGroupedAsync(null, null, null, null, "year")).ToList();

            result.Should().HaveCount(2);

            var year2000 = result.Single(r => r.Year == 2000);
            year2000.Count.Should().Be(2);
            year2000.TotalMass.Should().Be(30);

            var year2001 = result.Single(r => r.Year == 2001);
            year2001.Count.Should().Be(1);
            year2001.TotalMass.Should().Be(30);
        }

        [Fact]
        public async Task FilterByYearRange()
        {
            await using var dbContext = _fixture.GetDbContext();

            var recclassId = Guid.NewGuid();

            dbContext.Recclasses.Add(new Recclass
            {
                ClassId = recclassId,
                RecclassName = "test"
            });
            dbContext.Meteorites.AddRange(
                CreateMeteorite("1", "A", 2000, 10, recclassId),
                CreateMeteorite("2", "B", 2000, 20, recclassId),
                CreateMeteorite("3", "C", 2001, 30, recclassId)
            );

            await dbContext.SaveChangesAsync();

            var sut = new MeteoriteRepository(dbContext);

            var result = (await sut.GetGroupedAsync(2000, 2000, null, null, "year")).ToList();

            result.Should().HaveCount(1);
            result.Single().Year.Should().Be(2000);
        }

        [Fact]
        public async Task FilterByRecclass()
        {
            await using var dbContext = _fixture.GetDbContext();

            var recclass1 = Guid.NewGuid();
            var recclass2 = Guid.NewGuid();

            dbContext.Recclasses.AddRange(
                new Recclass
                {
                    ClassId = recclass1,
                    RecclassName = "test1"
                }, new Recclass
                {
                    ClassId = recclass2,
                    RecclassName = "test2"
                });
            dbContext.Meteorites.AddRange(
                CreateMeteorite("1", "A", 2000, 10, recclass1),
                CreateMeteorite("2", "B", 2000, 20, recclass2)
            );

            await dbContext.SaveChangesAsync();

            var sut = new MeteoriteRepository(dbContext);

            var result = (await sut.GetGroupedAsync(null, null, recclass1, null, "year")).ToList();

            result.Should().HaveCount(1);
            result.Single().TotalMass.Should().Be(10);
        }

        [Fact]
        public async Task FilterByNameContains()
        {
            await using var dbContext = _fixture.GetDbContext();

            var recclassId = Guid.NewGuid();
            dbContext.Recclasses.Add(new Recclass
            {
                ClassId = recclassId,
                RecclassName = "test"
            });
            dbContext.Meteorites.AddRange(
                CreateMeteorite("1", "Alpha", 2000, 10, recclassId),
                CreateMeteorite("2", "Beta", 2000, 20, recclassId)
            );

            await dbContext.SaveChangesAsync();

            var sut = new MeteoriteRepository(dbContext);

            var result = (await sut.GetGroupedAsync(null, null, null, "Al", "year")).ToList();

            result.Should().HaveCount(1);
            result.Single().TotalMass.Should().Be(10);
        }

        [Theory]
        [InlineData("count", true)]
        [InlineData("count", false)]
        [InlineData("mass", true)]
        [InlineData("mass", false)]
        [InlineData("year", true)]
        [InlineData("year", false)]
        public async Task SortByDifferentFields(string sortBy, bool ascending)
        {
            await using var dbContext = _fixture.GetDbContext();

            var recclassId = Guid.NewGuid();
            dbContext.Recclasses.Add(new Recclass
            {
                ClassId = recclassId,
                RecclassName = "test"
            });
            dbContext.Meteorites.AddRange(
                CreateMeteorite("1", "A", 2000, 10, recclassId),
                CreateMeteorite("2", "B", 2000, 20, recclassId),
                CreateMeteorite("3", "C", 2001, 30, recclassId)
            );

            await dbContext.SaveChangesAsync();

            var sut = new MeteoriteRepository(dbContext);

            var result = (await sut.GetGroupedAsync(null, null, null, null, sortBy, ascending)).ToList();

            result.Should().NotBeEmpty();
        }
    }
}
