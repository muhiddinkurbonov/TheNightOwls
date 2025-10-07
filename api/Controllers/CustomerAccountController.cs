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
    public class CustomerAccountController(
        ICustomerAppointmentService _customerAppointmentService,
        IUserAccountService _userAccountService
    ) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IsTakenDTO>> CheckUsername([FromBody] NameDTO nameDTO)
        {
            var result = await _userAccountService.CheckIfUsernameExistsAsync(nameDTO.Name);
            return Ok(new IsTakenDTO { IsTaken=result });
        }


        /*
        Task<CustomerModel> LoginAsync(string username);
        Task<bool> CheckIfUsernameExistsAsync(string username);
        Task<CustomerModel> SignUpCustomerAsync(CustomerModel customerModel);
        Task<CustomerModel> GetCustomerByIdAsync(int customerId);
        */
    }
}