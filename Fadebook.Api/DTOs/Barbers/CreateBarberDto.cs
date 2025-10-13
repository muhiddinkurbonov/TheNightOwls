using System.ComponentModel.DataAnnotations;

namespace Fadebook.DTOs.Barbers;

public class CreateBarberDto
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Username must be between 1 and 50 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 50 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Specialty must not exceed 100 characters.")]
    public string Specialty { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Contact info must not exceed 50 characters.")]
    public string ContactInfo { get; set; } = string.Empty;

    // List of service IDs to associate with this barber
    public List<int> ServiceIds { get; set; } = new List<int>();
}
