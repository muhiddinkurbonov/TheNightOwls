using AutoMapper;
using Fadebook.DTOs.Appointments;
using Fadebook.DTOs.Auth;
using Fadebook.DTOs.Barbers;
using Fadebook.DTOs.Services;
using Fadebook.DTOs.Customers;
using Fadebook.DTOs.Common;
using Fadebook.Models;
using Fadebook.Controllers;

namespace Fadebook.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<BarberModel, BarberDto>().ReverseMap();
        CreateMap<CreateBarberDto, BarberModel>();
        CreateMap<CustomerModel, CustomerDto>().ReverseMap();
        CreateMap<ServiceModel, ServiceDto>().ReverseMap();

        // Appointment mapping with navigation property names
        CreateMap<AppointmentModel, AppointmentDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : null))
            .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src => src.Barber != null ? src.Barber.Name : null))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.ServiceName : null));

        // Reverse mapping - ignore navigation properties when mapping from DTO to Model
        CreateMap<AppointmentDto, AppointmentModel>()
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Barber, opt => opt.Ignore())
            .ForMember(dest => dest.Service, opt => opt.Ignore());

        // Auth mappings
        CreateMap<UserModel, UserDto>().ReverseMap();

        // BarberWorkHours mappings
        CreateMap<BarberWorkHoursModel, BarberWorkHoursDto>()
            .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src => src.Barber != null ? src.Barber.Name : ""))
            .ForMember(dest => dest.DayOfWeekName, opt => opt.MapFrom(src => GetDayOfWeekName(src.DayOfWeek)))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString("HH:mm")))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("HH:mm")));

        CreateMap<CreateBarberWorkHoursDto, BarberWorkHoursModel>()
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TimeOnly.Parse(src.StartTime)))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => TimeOnly.Parse(src.EndTime)))
            .ForMember(dest => dest.Barber, opt => opt.Ignore())
            .ForMember(dest => dest.WorkHourId, opt => opt.Ignore());
    }

    private static string GetDayOfWeekName(int dayOfWeek)
    {
        return dayOfWeek switch
        {
            0 => "Sunday",
            1 => "Monday",
            2 => "Tuesday",
            3 => "Wednesday",
            4 => "Thursday",
            5 => "Friday",
            6 => "Saturday",
            _ => "Unknown"
        };
    }
}
