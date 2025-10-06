using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Services;
using Fadebook.DTOs;
using AutoMapper;

namespace Fadebook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // "/students
    public class CustomerAppointmentController : ControllerBase
    {
        // Fields
        private readonly ILogger<CustomerAppointmentController> _logger;
        private readonly ICustomerAppointmentService _service;
        private readonly IMapper _mapper;

        // Constructor
        public CustomerAppointmentController(ILogger<CustomerAppointmentController> logger, ICustomerAppointmentService service, IMapper mapper)
        {
            _logger = logger;
            _service = service;
            _mapper = mapper;
        }

        // Task<AppointmentModel> RequestAppointmentAsync(CustomerModel customer, AppointmentModel appointment);
        // //getBarberByService
        // Task<IEnumerable<BarberModel>> GetBarbersByServiceAsync(int serviceId);
        // //getServices/*
        // Task<IEnumerable<ServiceModel>> GetServicesAsync();

        // Methods

        // Enroll Student In Course
        // [Authorize(Roles = "Student")]
        // [Authorize(Roles = "Instructor")]
        // /CustomerAppointment/RequestAppointment
        // [HttpPost("/RequestAppointment", Name = "EnrollStudentInCourse")] // Name lets us short hand out the endpoint for use by our code somehow
        // public async Task<IActionResult> RequestAppointment([FromBody] AppointmentDto appointmentDt0)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     try{
        //         var appointment = await _service.RequestAppointmentAsync(appointmentDt0.CustomerId, appointmentDt0.AppointmentId);
        //         return Ok(appointment);
        //     }
        //     catch(Exception ex)
        //     {
        //         _logger.LogError(ex, "Error requesting appointment");
        //         return StatusCode(500, "Internal server error");
        //     }


        // }

        // POST: api/customerappointment
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create([FromBody] CustomerDto customerDto)
        {
            var customer = _mapper.Map<CustomerModel>(customerDto);
            var createdCustomer = await _service.AddCustomerAsync(customer);
            var dto = _mapper.Map<CustomerDto>(createdCustomer);
            return CreatedAtAction("GetCustomerById", new { id = createdCustomer.CustomerId }, dto);
        }

        // GET: /customer/{id}
        [HttpGet("/customer/{id}", Name = "GetCustomerById")]
        public async Task<ActionResult<CustomerDto>> GetById(int id)
        {
            var customer = await _service.GetCustomerByIdAsync(id);
            if (customer == null) 
                return NotFound(new { message = $"Customer with ID {id} not found." });
            
            return Ok(_mapper.Map<CustomerDto>(customer));
        }

        // GET: api/customerappointment/services
        [HttpGet("services")]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
        {
            var services = await _service.GetServicesAsync();
            var dtos = _mapper.Map<IEnumerable<ServiceDto>>(services);
            return Ok(dtos);
        }

        // GET: api/customerappointment/barbers-by-service/{serviceId}
        [HttpGet("barbers-by-service/{serviceId:int}")]
        public async Task<ActionResult<IEnumerable<BarberDto>>> GetBarbersByService([FromRoute] int serviceId)
        {
            var barbers = await _service.GetBarbersByServiceAsync(serviceId);
            var dtos = _mapper.Map<IEnumerable<BarberDto>>(barbers);
            return Ok(dtos);
        }

        // POST: api/customerappointment/request-appointment
        [HttpPost("request-appointment")]
        public async Task<ActionResult<AppointmentDto>> RequestAppointment([FromBody] AppointmentRequestDto request)
        {
            var customer = _mapper.Map<CustomerModel>(request.Customer);
            var appointment = _mapper.Map<AppointmentModel>(request.Appointment);
            var created = await _service.RequestAppointmentAsync(customer, appointment);
            var dto = _mapper.Map<AppointmentDto>(created);
            return Created($"/api/appointment/{created.AppointmentId}", dto);
        }


    }
}