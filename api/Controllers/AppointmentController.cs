using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Services;
using AutoMapper;
using Fadebook.DTOs;

namespace Fadebook.Controllers;

[ApiController]
[Route("api/[controller]")]
// api/appointment
public class AppointmentController : ControllerBase
{
    // Fields
    private readonly IAppointmentManagementService _service;
    private readonly IMapper _mapper;

    // Constructor
    public AppointmentController(IAppointmentManagementService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    // GET: api/appointment
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
    {
        var appointments = await _service.GetAllAppointmentsAsync();
        var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        return Ok(dtos);
    }

    // POST: api/appointment
    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> Create([FromBody] AppointmentDto appointmentDto)
    {
        try
        {
            var model = _mapper.Map<AppointmentModel>(appointmentDto);
            var created = await _service.AddAppointmentAsync(model);

            if (created == null)
            {
                return Conflict(new { message = "Failed to create appointment. Invalid customer, barber, or service ID." });
            }

            var dto = _mapper.Map<AppointmentDto>(created);
            return Created($"api/appointment/{created.AppointmentId}", dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the appointment.", error = ex.Message });
        }
    }

    // GET: api/appointment/{id}
    [HttpGet("{id:int}", Name = "GetAppointmentById")]
    public async Task<ActionResult<AppointmentDto>> GetById([FromRoute] int id)
    {
        var appt = await _service.GetAppointmentByIdAsync(id);
        return Ok(_mapper.Map<AppointmentDto>(appt));
    }

    // PUT: api/appointment/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Barber,Customer")]
    public async Task<ActionResult<AppointmentDto>> Update([FromRoute] int id, [FromBody] AppointmentDto appointmentDto)
    {
        var model = _mapper.Map<AppointmentModel>(appointmentDto);
        model.AppointmentId = id;
        var updated = await _service.UpdateAppointmentAsync(id, model);
        return Ok(_mapper.Map<AppointmentDto>(updated));
    }

    // GET: api/appointment/by-date?date=2025-01-01
    [HttpGet("by-date")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByDate([FromQuery] DateTime date)
    {
        var appts = await _service.GetAppointmentsByDateAsync(date);
        var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appts);
        return Ok(dtos);
    }

    // GET: api/appointment/by-username/{username}
    [HttpGet("by-username/{username}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByUsername([FromRoute] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new { message = "Username is required." });
        var appts = await _service.LookupAppointmentsByUsernameAsync(username);
        var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appts);
        return Ok(dtos);
    }

    // GET: api/appointment/by-barber/{barberId}
    [HttpGet("by-barber/{barberId:int}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByBarberId([FromRoute] int barberId)
    {
        var appts = await _service.GetAppointmentsByBarberIdAsync(barberId);
        var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appts);
        return Ok(dtos);
    }

    // DELETE: api/appointment/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var result = await _service.DeleteAppointmentAsync(id);
        return NoContent();
    }

}
