using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NasaSyncService.Application.Interfaces.Repositories;
using NasaSyncService.Infrastructure.Data.Entities;
using NasaSyncService.Infrastructure.Mappings;
using NasaSyncService.Infrastructure.Repositories;

namespace NasaSyncService.Test.Units.Infrasctructure.Repositories
{
    public class RecclassRepositoryShould(InfrastructureTestFixture fixture) : IClassFixture<InfrastructureTestFixture>
    {
        private readonly InfrastructureTestFixture _fixture = fixture;
        private readonly IMapper _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<RecclassProfile>(), new NullLoggerFactory()));

        [Fact]
        public async Task ReturnEmptyList_WhenNoRecclassesExist()
        {
            await using var dbContext = _fixture.GetDbContext();
            var sut = new RecclassRepository(dbContext, _mapper);

            var result = await sut.GetRecclassesAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ReturnRecclasses_WhenTheyExist()
        {
            await using var dbContext = _fixture.GetDbContext();

            dbContext.Recclasses.Add(new Recclass
            {
                ClassId = Guid.Parse("f4c9e6de-2511-4352-b3a8-f3253b328daa"),
                RecclassName = "H5"
            });
            dbContext.Recclasses.Add(new Recclass
            {
                ClassId = Guid.Parse("4cba33da-d770-49f6-9bd0-4db50851439a"),
                RecclassName = "L6"
            });

            await dbContext.SaveChangesAsync();

            var sut = new RecclassRepository(dbContext, _mapper);

            var result = (await sut.GetRecclassesAsync()).ToList();

            result.Should().HaveCount(2);
            result.Select(r => r.ClassName).Should().Contain(["H5", "L6"]);
        }
    }
}
