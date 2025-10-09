using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Services;
using AutoMapper;
using Fadebook.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Fadebook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // "/students
    public class CustomerAccountController(
        IUserAccountService _userAccountService,
        IMapper _mapper 
    ) : ControllerBase
    {
        [HttpPost("signup")]
        public async Task<ActionResult<CustomerDto>> SignUp([FromBody] CustomerDto customerDto)
        {
            try
            {
                var customer = _mapper.Map<CustomerModel>(customerDto);
                var createdCustomer = await _userAccountService.SignUpCustomerAsync(customer);
                var dto = _mapper.Map<CustomerDto>(createdCustomer);
                return Created($"/customer/{createdCustomer.CustomerId}", dto);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/customerappointment/login
        [HttpPost("login")]
        public async Task<ActionResult<CustomerDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                var customer = await _userAccountService.LoginAsync(loginRequest.Username);
                var dto = _mapper.Map<CustomerDto>(customer);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Username not found. Please sign up first." });
            }
        }


        /*
        Task<CustomerModel> LoginAsync(string username);
        Task<bool> CheckIfUsernameExistsAsync(string username);
        Task<CustomerModel> SignUpCustomerAsync(CustomerModel customerModel);
        Task<CustomerModel> GetCustomerByIdAsync(int customerId);
        */
    }
}