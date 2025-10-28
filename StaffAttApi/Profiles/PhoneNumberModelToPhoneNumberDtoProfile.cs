using AutoMapper;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Profiles;

public class PhoneNumberModelToPhoneNumberDtoProfile : Profile
{
    public PhoneNumberModelToPhoneNumberDtoProfile()
    {
        CreateMap<PhoneNumberModel, PhoneNumberDto>()
            .ForPath(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForPath(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ReverseMap();
    }
}
