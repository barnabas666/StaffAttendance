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
            .ForPath(dest => dest.Address.Street, opt => opt.MapFrom(src => src.Address.Street))
            .ForPath(dest => dest.Address.City, opt => opt.MapFrom(src => src.Address.City))
            .ForPath(dest => dest.Address.Zip, opt => opt.MapFrom(src => src.Address.Zip))
            .ForPath(dest => dest.Address.State, opt => opt.MapFrom(src => src.Address.State))
            .ReverseMap();
    }
}
