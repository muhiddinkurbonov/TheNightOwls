
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheNightOwls.Models;

public class BarberModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BarberId { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    // Specialty or service area (e.g., 'Fades', 'Beard Trims', 'Women's Cuts')
    [StringLength(100)]
    public string Specialty { get; set; }

    // Contact Information (e.g., phone number, email)
    [StringLength(255)]
    public string ContactInfo { get; set; }

    // public ICollection<Appointment> Appointments { get; set; }
}