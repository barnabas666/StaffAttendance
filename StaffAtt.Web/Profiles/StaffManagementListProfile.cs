using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Profiles;

public class StaffManagementListProfile : Profile
{
    public StaffManagementListProfile()
    {
        CreateMap<StaffBasicModel, StaffManagementListViewModel>()
            .ForPath(dest => dest.BasicInfo.Id, opt => opt.MapFrom(src => src.Id))
            .ForPath(dest => dest.BasicInfo.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForPath(dest => dest.BasicInfo.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForPath(dest => dest.BasicInfo.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForPath(dest => dest.BasicInfo.Alias, opt => opt.MapFrom(src => src.Alias))
            .ForPath(dest => dest.BasicInfo.Title, opt => opt.MapFrom(src => src.Title))
            .ForPath(dest => dest.BasicInfo.IsApproved, opt => opt.MapFrom(src => src.IsApproved))
            .ReverseMap();
    }
}
