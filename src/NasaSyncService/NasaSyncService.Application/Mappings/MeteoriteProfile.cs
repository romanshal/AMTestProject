using AutoMapper;
using NasaSyncService.Application.Dtos;
using NasaSyncService.Domain.Models;

namespace NasaSyncService.Application.Mappings
{
    internal class MeteoriteProfile : Profile
    {
        public MeteoriteProfile()
        {
            CreateMap<GroupedMeteoriteDomain, GroupedMeteoritesDto>();
        }
    }
}
