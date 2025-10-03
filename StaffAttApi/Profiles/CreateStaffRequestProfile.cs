using AutoMapper;
using StaffAttShared.DTOs;
using StaffAttLibrary.Models;

namespace StaffAttApi.Profiles;

public class CreateStaffRequestProfile : Profile
{
    public CreateStaffRequestProfile()
    {
        CreateMap<CreateStaffRequest, (int DepartmentId, AddressModel Address, string PIN, string FirstName, string LastName, string EmailAddress, List<PhoneNumberModel> PhoneNumbers)>()
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.PIN, opt => opt.MapFrom(src => src.PIN))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForMember(dest => dest.PhoneNumbers, opt => opt.MapFrom(src => src.PhoneNumbers));
    }
}
