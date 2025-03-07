﻿using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Profiles;

public class CheckInFullProfile : Profile
{
    public CheckInFullProfile()
    {
        CreateMap<CheckInFullModel, CheckInFullViewModel>()
            .ForPath(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForPath(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForPath(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForPath(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForPath(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForPath(dest => dest.CheckInDate, opt => opt.MapFrom(src => src.CheckInDate))
            .ForPath(dest => dest.CheckOutDate, opt => opt.MapFrom(src => src.CheckOutDate))
            .ReverseMap();
    }
}
