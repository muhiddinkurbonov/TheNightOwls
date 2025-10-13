namespace Fadebook.DTOs.BarberWorkHours;

public class CreateBarberWorkHoursDto
{
    public int BarberId { get; set; }
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
