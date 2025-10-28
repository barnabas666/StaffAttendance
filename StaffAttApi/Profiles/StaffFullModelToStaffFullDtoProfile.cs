using AutoMapper;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Profiles;

public class StaffFullModelToStaffFullDtoProfile : Profile
{
    public StaffFullModelToStaffFullDtoProfile()
    {
        CreateMap<StaffFullModel, StaffFullDto>()
            .ForPath(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForPath(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForPath(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForPath(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForPath(dest => dest.Alias, opt => opt.MapFrom(src => src.Alias))
            .ForPath(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved))
            .ForPath(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
            .ForPath(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForPath(dest => dest.AddressId, opt => opt.MapFrom(src => src.AddressId))
            .ForPath(dest => dest.AliasId, opt => opt.MapFrom(src => src.AliasId))
            .ForPath(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForPath(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.PhoneNumbers, opt => opt.MapFrom(src => src.PhoneNumbers))
            .ReverseMap();
    }
}
