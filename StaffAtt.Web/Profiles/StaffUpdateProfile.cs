using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Profiles;

public class StaffUpdateProfile : Profile
{
    public StaffUpdateProfile()
    {
        CreateMap<StaffFullModel, StaffUpdateViewModel>()
            .ForPath(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForPath(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForPath(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForPath(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
            .ForPath(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            .ForPath(dest => dest.Zip, opt => opt.MapFrom(src => src.Address.Zip))
            .ForPath(dest => dest.State, opt => opt.MapFrom(src => src.Address.State))
            .ReverseMap();
    }
}
