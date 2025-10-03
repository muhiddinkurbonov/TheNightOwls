
namespace Fadebook.DTOs;

public class AppointmentDto
{
    public int AppointmentId { get; set; }
    public string Status { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public int BarberId { get; set; }
    public DateTime appointmentDate { get; set; }
}

