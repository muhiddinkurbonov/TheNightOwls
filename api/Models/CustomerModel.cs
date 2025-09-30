
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fadebook.Models;

public class CustomerModel
{
    [Key]
    public int CustomerId { get; set; }
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string? ContactInfo { get; set; }
    // public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}