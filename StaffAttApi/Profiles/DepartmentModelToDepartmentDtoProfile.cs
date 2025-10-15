using AutoMapper;
using StaffAttShared.DTOs;
using StaffAttLibrary.Models;

namespace StaffAttApi.Profiles;

public class DepartmentModelToDepartmentDtoProfile : Profile
{
    public DepartmentModelToDepartmentDtoProfile()
    {        
        CreateMap<DepartmentModel, DepartmentDto>()
            .ForPath(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForPath(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForPath(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ReverseMap();
    }
}
