using AutoMapper;
using StaffAttShared.DTOs;
using StaffAttLibrary.Models;

namespace StaffAttApi.Profiles;

public class PhoneNumberDtoProfile : Profile
{
    public PhoneNumberDtoProfile()
    {
        CreateMap<PhoneNumberModel, PhoneNumberDto>()
            .ForPath(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForPath(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ReverseMap();
    }
}
