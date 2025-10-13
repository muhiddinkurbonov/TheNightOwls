namespace Fadebook.DTOs.BarberWorkHours;

public class BarberWorkHoursDto
{
    public int WorkHourId { get; set; }
    public int BarberId { get; set; }
    public string BarberName { get; set; } = string.Empty;
    public int DayOfWeek { get; set; }
    public string DayOfWeekName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
