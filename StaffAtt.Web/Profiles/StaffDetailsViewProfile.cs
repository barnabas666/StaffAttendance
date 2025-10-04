using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Profiles;

public class StaffDetailsViewProfile : Profile
{
    public StaffDetailsViewProfile()
    {
        CreateMap<StaffFullDto, StaffDetailsViewModel>()
            .ForPath(dest => dest.BasicInfo.Id, opt => opt.MapFrom(src => src.Id))
            .ForPath(dest => dest.BasicInfo.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForPath(dest => dest.BasicInfo.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForPath(dest => dest.BasicInfo.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForPath(dest => dest.BasicInfo.Alias, opt => opt.MapFrom(src => src.Alias))
            .ForPath(dest => dest.BasicInfo.IsApproved, opt => opt.MapFrom(src => src.IsApproved))
            .ForPath(dest => dest.BasicInfo.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
            .ForPath(dest => dest.BasicInfo.Title, opt => opt.MapFrom(src => src.Title))
            .ForPath(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.PhoneNumbers, opt => opt.MapFrom(src => src.PhoneNumbers))
            .ReverseMap();
    }
}
