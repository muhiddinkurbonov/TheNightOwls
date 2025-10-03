using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Services;
using Fadebook.DTOs;
using Serilog;

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

        // Constructor
        public CustomerAppointmentController(ILogger<CustomerAppointmentController> logger, ICustomerAppointmentService service)
        {
            _logger = logger;
            _service = service;
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

        [HttpPost]
        public async Task<ActionResult<CustomerModel>> CreateCustomer([FromBody] CustomerModel customer)
        {
            if (customer == null)
                return BadRequest();

            var createdCustomer = await _service.AddCustomerAsync(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.CustomerId }, createdCustomer);
        }

        [HttpGet("/customer/{id}")]
        public async Task<ActionResult<CustomerModel>> GetCustomerById(int id)
        {

            var customer = await _service.GetCustomerByIdAsync(id);
            if(customer == null) 
                return NotFound();
            return Ok(customer);
        }

        // GET: api/customer/services
        //Utlized for making an appointment
        [HttpGet("services")]
        public async Task<ActionResult<IEnumerable<ServiceModel>>> GetServices()
        {
            var services = await _service.GetServicesAsync();
            return Ok(services);
        }

        // GET: api/customer/barbers-by-service/{serviceId}
        [HttpGet("barbers-by-service/{serviceId:int}")]
        public async Task<ActionResult<IEnumerable<BarberModel>>> GetBarbersByService([FromRoute] int serviceId)
        {
            var barbers = await _service.GetBarbersByServiceAsync(serviceId);
            return Ok(barbers);
        }

        public class AppointmentRequestDto
        {
            public CustomerModel Customer { get; set; }
            public AppointmentModel Appointment { get; set; }
        }

        // POST: api/customer/request-appointment
        [HttpPost("request-appointment")]
        public async Task<ActionResult<AppointmentModel>> RequestAppointment([FromBody] AppointmentRequestDto request)
        {
            if (request is null || request.Customer is null || request.Appointment is null)
                return BadRequest("Customer and Appointment are required.");

            var created = await _service.RequestAppointmentAsync(request.Customer, request.Appointment);
            return Ok(created);
        }


    }
}