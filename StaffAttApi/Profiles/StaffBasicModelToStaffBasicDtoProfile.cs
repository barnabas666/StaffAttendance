using AutoMapper;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Profiles;

public class StaffBasicModelToStaffBasicDtoProfile : Profile
{
    public StaffBasicModelToStaffBasicDtoProfile()
    {
        CreateMap<StaffBasicModel, StaffBasicDto>()
            .ForPath(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForPath(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForPath(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForPath(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForPath(dest => dest.Alias, opt => opt.MapFrom(src => src.Alias))
            .ForPath(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved))
            .ForPath(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
            .ForPath(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ReverseMap();
    }
}
