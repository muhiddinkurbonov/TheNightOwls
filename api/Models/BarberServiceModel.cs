
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheNightOwls.Models;

public class BarberServiceModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BarberId { get; set; } // Foreign key
    [ForeignKey(nameof(BarberId))]
    public BarberModel Barber { get; set; }
    
    [Required]
    public int ServiceId { get; set; } // Foreign key
    [ForeignKey(nameof(ServiceId))]
    public ServiceModel Service { get; set; }
}