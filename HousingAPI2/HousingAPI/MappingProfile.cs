using AutoMapper;
using HousingAPI.Dtos;
using HousingAPI.Models;

namespace HousingAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // House mappings
            CreateMap<House, HouseDto>().ReverseMap();

            // Apartment mappings
            CreateMap<Apartment, ApartmentDto>()
                .ForMember(dest => dest.HouseLink, opt => opt.Ignore())
                .ForMember(dest => dest.Residents, opt => opt.MapFrom(src => src.Residents))
                .ReverseMap()
                .ForMember(dest => dest.House, opt => opt.Ignore())
                .ForMember(dest => dest.Residents, opt => opt.Ignore());

            // Resident mappings
            CreateMap<Resident, ResidentDto>()
                .ForMember(dest => dest.ApartmentLink, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Apartment, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}