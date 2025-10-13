using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Fadebook.Models;
using Fadebook.Services;
using Fadebook.DTOs.BarberWorkHours;

namespace Fadebook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarberWorkHoursController : ControllerBase
    {
        private readonly IBarberWorkHoursService _service;
        private readonly IMapper _mapper;

        public BarberWorkHoursController(IBarberWorkHoursService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // GET: api/BarberWorkHours
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BarberWorkHoursDto>>> GetAll()
        {
            var result = await _service.GetAllWorkHoursAsync();
            var dtos = _mapper.Map<IEnumerable<BarberWorkHoursDto>>(result);
            return Ok(dtos);
        }

        // GET: api/BarberWorkHours/{id}
        [HttpGet("{id:int}", Name = "GetWorkHoursById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BarberWorkHoursDto>> GetById([FromRoute] int id)
        {
            var result = await _service.GetWorkHoursByIdAsync(id);
            return Ok(_mapper.Map<BarberWorkHoursDto>(result));
        }

        // GET: api/BarberWorkHours/barber/{barberId}
        [HttpGet("barber/{barberId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BarberWorkHoursDto>>> GetByBarberId([FromRoute] int barberId)
        {
            var result = await _service.GetWorkHoursByBarberIdAsync(barberId);
            var dtos = _mapper.Map<IEnumerable<BarberWorkHoursDto>>(result);
            return Ok(dtos);
        }

        // GET: api/BarberWorkHours/barber/{barberId}/day/{dayOfWeek}
        [HttpGet("barber/{barberId:int}/day/{dayOfWeek:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<BarberWorkHoursDto>>> GetByBarberIdAndDay(
            [FromRoute] int barberId,
            [FromRoute] int dayOfWeek)
        {
            var result = await _service.GetWorkHoursByBarberIdAndDayAsync(barberId, dayOfWeek);
            var dtos = _mapper.Map<IEnumerable<BarberWorkHoursDto>>(result);
            return Ok(dtos);
        }

        // GET: api/BarberWorkHours/barber/{barberId}/available
        [HttpGet("barber/{barberId:int}/available")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> IsBarberAvailable(
            [FromRoute] int barberId,
            [FromQuery] DateTime appointmentDateTime,
            [FromQuery] int durationMinutes = 30)
        {
            var isAvailable = await _service.IsBarberAvailableAsync(barberId, appointmentDateTime, durationMinutes);
            return Ok(new { isAvailable });
        }

        // GET: api/BarberWorkHours/barber/{barberId}/slots
        [HttpGet("barber/{barberId:int}/slots")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetAvailableTimeSlots(
            [FromRoute] int barberId,
            [FromQuery] DateTime date,
            [FromQuery] int durationMinutes = 30)
        {
            var slots = await _service.GetAvailableTimeSlotsAsync(barberId, date, durationMinutes);
            return Ok(slots);
        }

        // POST: api/BarberWorkHours
        [HttpPost]
        [Authorize(Roles = "Admin,Barber")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BarberWorkHoursDto>> Create([FromBody] CreateBarberWorkHoursDto dto)
        {
            var model = _mapper.Map<BarberWorkHoursModel>(dto);
            var created = await _service.AddWorkHoursAsync(model);
            var createdDto = _mapper.Map<BarberWorkHoursDto>(created);
            return CreatedAtRoute("GetWorkHoursById", new { id = created.WorkHourId }, createdDto);
        }

        // PUT: api/BarberWorkHours/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Barber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BarberWorkHoursDto>> Update(
            [FromRoute] int id,
            [FromBody] CreateBarberWorkHoursDto dto)
        {
            var model = _mapper.Map<BarberWorkHoursModel>(dto);
            var updated = await _service.UpdateWorkHoursAsync(id, model);
            return Ok(_mapper.Map<BarberWorkHoursDto>(updated));
        }

        // DELETE: api/BarberWorkHours/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Barber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BarberWorkHoursDto>> Delete([FromRoute] int id)
        {
            var deleted = await _service.DeleteWorkHoursAsync(id);
            return Ok(_mapper.Map<BarberWorkHoursDto>(deleted));
        }
    }
}
