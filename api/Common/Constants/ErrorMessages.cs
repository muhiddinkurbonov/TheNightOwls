namespace Fadebook.Common.Constants;

/// <summary>
/// Constants for common error messages
/// </summary>
public static class ErrorMessages
{
    // Authentication & Authorization
    public const string InvalidCredentials = "Invalid username or password.";
    public const string UserNotFound = "User not found.";
    public const string Unauthorized = "You are not authorized to perform this action.";
    public const string TokenExpired = "Your session has expired. Please log in again.";

    // Appointments
    public const string AppointmentNotFound = "Appointment not found.";
    public const string InvalidAppointmentDate = "Invalid appointment date.";
    public const string AppointmentConflict = "The selected time slot is not available.";

    // Barbers
    public const string BarberNotFound = "Barber not found.";
    public const string BarberNotAvailable = "Barber is not available at the selected time.";

    // Services
    public const string ServiceNotFound = "Service not found.";

    // Customers
    public const string CustomerNotFound = "Customer not found.";

    // Validation
    public const string RequiredField = "This field is required.";
    public const string InvalidFormat = "Invalid format.";
    public const string InvalidDateRange = "End date must be after start date.";
}
