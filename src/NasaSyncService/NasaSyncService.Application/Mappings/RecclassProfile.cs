using AutoMapper;
using NasaSyncService.Application.Dtos;
using NasaSyncService.Domain.Models;

namespace NasaSyncService.Application.Mappings
{
    internal class RecclassProfile : Profile
    {
        public RecclassProfile()
        {
            CreateMap<RecclassDomain, RecclassDto>();
        }
    }
}
