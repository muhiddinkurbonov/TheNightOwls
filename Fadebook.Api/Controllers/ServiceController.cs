using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Services;
using Fadebook.DTOs.Services;
using AutoMapper;

namespace Fadebook.Controllers;

[ApiController]
[Route("api/[controller]")]
// api/service
public class ServiceController : ControllerBase
{
    private readonly IServiceManagementService _serviceService;
    private readonly IMapper _mapper;

    public ServiceController(
        IServiceManagementService serviceService,
        IMapper mapper)
    {
        _serviceService = serviceService;
        _mapper = mapper;
    }

    // GET: api/service
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAll()
    {
        var services = await _serviceService.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<ServiceDto>>(services);
        return Ok(dtos);
    }

    // GET: api/service/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceDto>> GetById([FromRoute] int id)
    {
        var service = await _serviceService.GetByIdAsync(id);
        var dto = _mapper.Map<ServiceDto>(service);
        return Ok(dto);
    }

    // POST: api/service
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceDto>> Create([FromBody] ServiceDto serviceDto)
    {
        var service = _mapper.Map<ServiceModel>(serviceDto);
        var created = await _serviceService.CreateAsync(service);
        var dto = _mapper.Map<ServiceDto>(created);
        return Created($"/api/service/{created.ServiceId}", dto);
    }

    // PUT: api/service/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceDto>> Update([FromRoute] int id, [FromBody] ServiceDto serviceDto)
    {
        var service = _mapper.Map<ServiceModel>(serviceDto);
        var updated = await _serviceService.UpdateAsync(id, service);
        var dto = _mapper.Map<ServiceDto>(updated);
        return Ok(dto);
    }

    // DELETE: api/service/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _serviceService.DeleteAsync(id);
        return NoContent();
    }
}
