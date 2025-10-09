using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.DB;
using Fadebook.DTOs;
using AutoMapper;

namespace Fadebook.Controllers;

[ApiController]
[Route("api/[controller]")]
// api/service
public class ServiceController : ControllerBase
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IDbTransactionContext _dbTransactionContext;
    private readonly IMapper _mapper;

    public ServiceController(
        IServiceRepository serviceRepository,
        IDbTransactionContext dbTransactionContext,
        IMapper mapper)
    {
        _serviceRepository = serviceRepository;
        _dbTransactionContext = dbTransactionContext;
        _mapper = mapper;
    }

    // GET: api/service
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAll()
    {
        var services = await _serviceRepository.GetAll();
        var dtos = _mapper.Map<IEnumerable<ServiceDto>>(services);
        return Ok(dtos);
    }

    // GET: api/service/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceDto>> GetById([FromRoute] int id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);
        if (service is null)
            return NotFound(new { message = $"Service with ID {id} not found." });

        var dto = _mapper.Map<ServiceDto>(service);
        return Ok(dto);
    }

    // POST: api/service
    [HttpPost]
    public async Task<ActionResult<ServiceDto>> Create([FromBody] ServiceDto serviceDto)
    {
        var service = _mapper.Map<ServiceModel>(serviceDto);
        await _serviceRepository.AddAsync(service);
        await _dbTransactionContext.SaveChangesAsync();

        var dto = _mapper.Map<ServiceDto>(service);
        return Created($"/api/service/{service.ServiceId}", dto);
    }

    // DELETE: api/service/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var service = await _serviceRepository.DeleteAsync(id);
        if (service is null)
            return NotFound(new { message = $"Service with ID {id} not found." });

        await _dbTransactionContext.SaveChangesAsync();
        return NoContent();
    }
}
