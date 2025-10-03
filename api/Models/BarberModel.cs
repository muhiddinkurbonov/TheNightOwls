
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fadebook.Models;

public class BarberModel
{
    [Key]
    // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BarberId { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, ErrorMessage = "Username must be between 1 and 50 characters.")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name must be between 1 and 50 characters.")]
    public string Name { get; set; } = "";

    // Specialty or service area (e.g., 'Fades', 'Beard Trims', 'Women's Cuts')
    [StringLength(100, ErrorMessage = "Name must be between 1 and 100 characters.")]
    public string Specialty { get; set; } = "";

    // Contact Information (e.g., phone number, email)
    [StringLength(50, ErrorMessage = "Name must be between 1 and 50 characters.")]
    public string ContactInfo { get; set; } = "";

    // public ICollection<Appointment> Appointments { get; set; }
}