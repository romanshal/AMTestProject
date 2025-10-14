using AutoMapper;
using NasaSyncService.Domain.Models;
using NasaSyncService.Infrastructure.Data.Entities;

namespace NasaSyncService.Infrastructure.Mappings
{
    public class RecclassProfile : Profile
    {
        public RecclassProfile()
        {
            CreateMap<Recclass, RecclassDomain>()
                .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.ClassId))
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.RecclassName));
        }
    }
}
