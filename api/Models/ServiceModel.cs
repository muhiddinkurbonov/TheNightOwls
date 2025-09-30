
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fadebook.Models;

public class ServiceModel
{
    [Key]
    public int ServiceId { get; set; }

    [Required]
    public string ServiceName { get; set; }

    [Required]
    [Range(0, 1000)]
    public double ServicePrice { get; set; }
}