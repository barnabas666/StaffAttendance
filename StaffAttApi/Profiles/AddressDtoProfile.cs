using AutoMapper;
using StaffAttShared.DTOs;
using StaffAttLibrary.Models;

namespace StaffAttApi.Profiles;

public class AddressDtoProfile : Profile
{
    public AddressDtoProfile()
    {        
        CreateMap<AddressModel, AddressDto>()
            .ForPath(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForPath(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
            .ForPath(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForPath(dest => dest.Zip, opt => opt.MapFrom(src => src.Zip))
            .ForPath(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ReverseMap();
    }
}
