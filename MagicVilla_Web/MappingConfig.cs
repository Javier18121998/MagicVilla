using AutoMapper;
using MagicVilla_Web.Models.DTOs;

namespace MagicVilla_Web
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<VillaDTO, VillaCreateDto>().ReverseMap();
            CreateMap<VillaDTO, VillaUpdateDto>().ReverseMap();
            CreateMap<NumeroVillaDto, NumeroVillaCreateDto>().ReverseMap();
            CreateMap<NumeroVillaDto, NumeroVillaUpdateDto>().ReverseMap();
        }
    }
}
