using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Services;
using Serilog;

namespace Fadebook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // "/students
    public class BarberController : ControllerBase
    {
        // Fields
        private readonly ILogger<BarberController> _logger;
        private readonly IBarberManagementService _service;

        public BarberController(IBarberManagementService service)
        {
            _service = service;
        }

        // Methods

        // Enroll Student In Course
        //[Authorize(Roles = "Student")]
        //[Authorize(Roles = "Instructor")]

        // GET: api/barber
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BarberModel>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET: api/barber/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BarberModel>> GetById([FromRoute] int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        // POST: api/barber
        [HttpPost]
        public async Task<ActionResult<BarberModel>> Create([FromBody] BarberModel model)
        {
            if (model is null) return BadRequest("Barber payload is required.");
            var created = await _service.AddAsync(model);
            return Ok(created);
        }

        // PUT: api/barber/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<BarberModel>> Update([FromRoute] int id, [FromBody] BarberModel model)
        {
            if (model is null) return BadRequest("Barber payload is required.");
            model.BarberId = id;
            var updated = await _service.UpdateAsync(model);
            if (updated is null) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/barber/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var ok = await _service.DeleteByIdAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // PUT: api/barber/{id}/services
        [HttpPut("{id:int}/services")]
        public async Task<ActionResult> UpdateServices([FromRoute] int id, [FromBody] List<int> serviceIds)
        {
            if (serviceIds is null) return BadRequest("serviceIds required");
            var ok = await _service.UpdateBarberServicesAsync(id, serviceIds);
            if (!ok) return NotFound();
            return NoContent();
        }


        
    }
}