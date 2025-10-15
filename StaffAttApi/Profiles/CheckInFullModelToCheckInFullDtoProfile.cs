using AutoMapper;
using StaffAttShared.DTOs;
using StaffAttLibrary.Models;

namespace StaffAttApi.Profiles;

public class CheckInFullModelToCheckInFullDtoProfile : Profile
{
    public CheckInFullModelToCheckInFullDtoProfile()
    {
        CreateMap<CheckInFullModel, CheckInFullDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
            .ForMember(dest => dest.CheckInDate, opt => opt.MapFrom(src => src.CheckInDate))
            .ForMember(dest => dest.CheckOutDate, opt => opt.MapFrom(src => src.CheckOutDate))
            .ReverseMap();
    }
}
