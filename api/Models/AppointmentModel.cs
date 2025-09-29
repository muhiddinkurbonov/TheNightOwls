
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheNightOwls.Models;

public class AppointmentModel
{
    [Key]
    public int AppointmentId { get; set; }
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, ErrorMessage = "Username must not exceed 100 characters")]
    public string Username { get; set; } = string.Empty;
    public DateTime Status { get; set; } = DateTime.Now;

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

}

