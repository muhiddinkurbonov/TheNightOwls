using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fadebook.DTOs.Auth;
using Fadebook.Repositories;
using AutoMapper;

namespace Fadebook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserController> _logger;

    public UserController(IAuthRepository authRepository, IMapper mapper, ILogger<UserController> logger)
    {
        _authRepository = authRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        _logger.LogInformation("Admin requesting all users");

        var users = await _authRepository.GetAllUsersAsync();
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        return Ok(userDtos);
    }
}
