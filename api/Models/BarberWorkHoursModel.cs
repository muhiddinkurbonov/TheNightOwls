using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fadebook.Models;

public class BarberWorkHoursModel : AModel
{
    [Key]
    public int WorkHourId { get; set; }

    [Required]
    public int BarberId { get; set; }

    [ForeignKey(nameof(BarberId))]
    public BarberModel Barber { get; set; } = null!;

    /// <summary>
    /// Day of week: 0 = Sunday, 1 = Monday, ..., 6 = Saturday
    /// </summary>
    [Required]
    [Range(0, 6, ErrorMessage = "DayOfWeek must be between 0 (Sunday) and 6 (Saturday).")]
    public int DayOfWeek { get; set; }

    /// <summary>
    /// Start time in HH:mm format (e.g., 09:00)
    /// </summary>
    [Required]
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// End time in HH:mm format (e.g., 17:00)
    /// </summary>
    [Required]
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Whether this work hour slot is currently active
    /// Can be set to false for holidays, vacation days, or sick days
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;
}
