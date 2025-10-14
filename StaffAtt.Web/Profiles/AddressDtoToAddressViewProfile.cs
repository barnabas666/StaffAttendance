using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Profiles;

public class AddressDtoToAddressViewProfile : Profile
{
    public AddressDtoToAddressViewProfile()
    {
        CreateMap<AddressDto, AddressViewModel>()
            .ForPath(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
            .ForPath(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForPath(dest => dest.Zip, opt => opt.MapFrom(src => src.Zip))
            .ForPath(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ReverseMap();
    }
}
