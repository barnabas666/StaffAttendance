using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Profiles;

public class StaffUpdateViewProfile : Profile
{
    public StaffUpdateViewProfile()
    {
        CreateMap<StaffFullDto, StaffUpdateViewModel>()
            .ForPath(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForPath(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForPath(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForPath(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ReverseMap();
    }
}
