using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Profiles;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<AddressModel, AddressViewModel>()
            .ForPath(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
            .ForPath(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForPath(dest => dest.Zip, opt => opt.MapFrom(src => src.Zip))
            .ForPath(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ReverseMap();
    }
}
