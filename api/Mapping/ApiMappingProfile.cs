using AutoMapper;
using Fadebook.DTOs;
using Fadebook.DTO;
using Fadebook.Models;

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
    }
}
