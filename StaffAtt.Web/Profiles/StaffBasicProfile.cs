using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Profiles;

public class StaffBasicProfile : Profile
{
    public StaffBasicProfile()
    {
        CreateMap<StaffBasicModel, StaffBasicViewModel>()
            .ForPath(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForPath(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForPath(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForPath(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForPath(dest => dest.Alias, opt => opt.MapFrom(src => src.Alias))
            .ForPath(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved))
            .ForPath(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ReverseMap();
    }
}
