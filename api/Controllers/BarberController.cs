using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Services;
using AutoMapper;
using Fadebook.DTOs;

namespace Fadebook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // "/students
    public class BarberController : ControllerBase
    {
        // Fields
        private readonly IBarberManagementService _service;
        private readonly IMapper _mapper;

        public BarberController(IBarberManagementService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // Methods

        // Enroll Student In Course
        //[Authorize(Roles = "Student")]
        //[Authorize(Roles = "Instructor")]

        // GET: api/barber
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BarberDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<BarberDto>>(result);
            return Ok(dtos);
        }

        // GET: api/barber/{id}
        [HttpGet("{id:int}", Name = "GetBarberById")]
        public async Task<ActionResult<BarberDto>> GetById([FromRoute] int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result is null) 
                return NotFound(new { message = $"Barber with ID {id} not found." });
            
            return Ok(_mapper.Map<BarberDto>(result));
        }

        // POST: api/barber
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<BarberDto>> Create([FromBody] BarberDto dto)
        {
            var model = _mapper.Map<BarberModel>(dto);
            var created = await _service.AddAsync(model);
            var createdDto = _mapper.Map<BarberDto>(created);
            return Created($"api/barber/{created.BarberId}", createdDto); 
        }

        // PUT: api/barber/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<BarberDto>> Update([FromRoute] int id, [FromBody] BarberDto dto)
        {
            var model = _mapper.Map<BarberModel>(dto);
            model.BarberId = id;
            var updated = await _service.UpdateAsync(model);
            if (updated is null) 
                return NotFound(new { message = $"Barber with ID {id} not found." });
            
            return Ok(_mapper.Map<BarberDto>(updated));
        }

        // DELETE: api/barber/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleted = await _service.DeleteByIdAsync(id);
            if (!deleted) 
                return NotFound(new { message = $"Barber with ID {id} not found." });
            
            return NoContent();
        }

        // PUT: api/barber/{id}/services
        [HttpPut("{id:int}/services")]
        public async Task<IActionResult> UpdateServices([FromRoute] int id, [FromBody] List<int> serviceIds)
        {
            if (serviceIds is null || !serviceIds.Any()) 
                return BadRequest(new { message = "Service IDs are required." });
            
            var updated = await _service.UpdateBarberServicesAsync(id, serviceIds);
            if (!updated) 
                return NotFound(new { message = $"Barber with ID {id} not found." });
            
            return NoContent();
        }


        
    }
}