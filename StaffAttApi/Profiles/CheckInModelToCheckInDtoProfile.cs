using AutoMapper;
using StaffAttShared.DTOs;
using StaffAttLibrary.Models;

namespace StaffAttApi.Profiles;

public class CheckInModelToCheckInDtoProfile : Profile
{
    public CheckInModelToCheckInDtoProfile()
    {
        CreateMap<CheckInModel, CheckInDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
            .ForMember(dest => dest.CheckInDate, opt => opt.MapFrom(src => src.CheckInDate))
            .ForMember(dest => dest.CheckOutDate, opt => opt.MapFrom(src => src.CheckOutDate))
            .ReverseMap();
    }
}
