
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fadebook.Models;

public class AppointmentModel
{
    // KEYS WILL BE PUBLIC IN DTO
    [Key]
    public int AppointmentId { get; set; }
    public string Status { get; set; } = "Pending";

    [Required]
    public int CustomerId { get; set; } // Foreign key
    [ForeignKey(nameof(CustomerId))]
    public CustomerModel Customer { get; set; }


    [Required]
    public int ServiceId { get; set; } // Foreign key
    [ForeignKey(nameof(ServiceId))]
    public ServiceModel Service { get; set; }

    [Required]
    public int BarberId { get; set; } // Foreign key
    [ForeignKey(nameof(BarberId))]
    public BarberModel Barber { get; set; }

    public DateTime appointmentDate { get; set; }
}

