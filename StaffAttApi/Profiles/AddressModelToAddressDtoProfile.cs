using AutoMapper;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Profiles;

public class AddressModelToAddressDtoProfile : Profile
{
    public AddressModelToAddressDtoProfile()
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
