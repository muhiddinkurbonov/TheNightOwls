using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Services;
using Serilog;

namespace Fadebook.Controllers;

[ApiController]
[Route("api/[controller]")]
// "/appointmentcontroller
public class AppointmentController : ControllerBase
{
    // Fields
    private readonly ILogger<AppointmentController> _logger;
    private readonly IAppointmentManagementService _service;

    // Constructor
    public AppointmentController(ILogger<AppointmentController> logger, IAppointmentManagementService service)
    {
        Console.WriteLine("Test!");
        _logger = logger;
        _service = service;
    }

    // Methods

    // [Authorize(Roles = "Customer")]
    // [Authorize(Roles = "Barber")]

    // POST: api/appointment
    [HttpPost]
    public async Task<IActionResult> AddAppointment(AppointmentModel appointmentModel)
    {
        _logger.LogInformation($"Adding appointment for {appointmentModel.CustomerId} with barber - {appointmentModel.BarberId}");
        var created = await _service.AddAppointment(appointmentModel);
        if (created is null) return Conflict("Unable to create appointment.");
        return Ok(created);
    }

    // PUT: api/appointment/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<AppointmentModel>> Update([FromRoute] int id, [FromBody] AppointmentModel appointmentModel)
    {
        if (appointmentModel is null) return BadRequest("Appointment payload is required.");
        appointmentModel.AppointmentId = id;
        var updated = await _service.UpdateAppointment(appointmentModel);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    // GET: api/appointment/by-date?date=2025-01-01
    [HttpGet("by-date")]
    public async Task<ActionResult<IEnumerable<AppointmentModel>>> GetByDate([FromQuery] DateTime date)
    {
        var appts = await _service.GetAppointmentsByDate(date);
        return Ok(appts);
    }

    // GET: api/appointment/by-username/{username}
    [HttpGet("by-username/{username}")]
    public async Task<ActionResult<IEnumerable<AppointmentModel>>> GetByUsername([FromRoute] string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest("username is required");
        var appts = await _service.LookupAppointmentsByUsername(username);
        return Ok(appts);
    }

    // DELETE: api/appointment/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<AppointmentModel>> Delete([FromRoute] int id)
    {
        var result = await _service.DeleteAppointment(new AppointmentModel { AppointmentId = id });
        if (result is null) return NotFound();
        return Ok(result);
    }

}
