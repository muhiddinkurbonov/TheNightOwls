using System.ComponentModel.DataAnnotations;
using Fadebook.DTOs.Customers;

namespace Fadebook.DTOs.Appointments;

public class AppointmentRequestDto
{
    [Required(ErrorMessage = "Customer information is required.")]
    public CustomerDto? Customer { get; set; }

    [Required(ErrorMessage = "Appointment information is required.")]
    public AppointmentDto? Appointment { get; set; }
}
