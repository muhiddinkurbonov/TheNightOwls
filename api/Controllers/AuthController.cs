using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Fadebook.DTOs.Auth;
using Fadebook.Services;

namespace Fadebook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        _logger.LogInformation("User registration attempt for username: {Username}", registerDto.Username);

        var result = await _authService.RegisterAsync(registerDto);

        _logger.LogInformation("User registered successfully: {Username}", registerDto.Username);

        return Ok(result);
    }

    /// <summary>
    /// Login with username/email and password
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation("User login attempt for: {UsernameOrEmail}", loginDto.UsernameOrEmail);

        var result = await _authService.LoginAsync(loginDto);

        _logger.LogInformation("User logged in successfully: {UsernameOrEmail}", loginDto.UsernameOrEmail);

        return Ok(result);
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid token.");
        }

        var user = await _authService.GetUserByIdAsync(userId);

        return Ok(user);
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<UserDto>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid token.");
        }

        _logger.LogInformation("Password change request for user ID: {UserId}", userId);

        var user = await _authService.ChangePasswordAsync(userId, changePasswordDto);

        _logger.LogInformation("Password changed successfully for user ID: {UserId}", userId);

        return Ok(user);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UserDto userDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid token.");
        }

        _logger.LogInformation("Profile update request for user ID: {UserId}", userId);

        var user = await _authService.UpdateUserAsync(userId, userDto);

        _logger.LogInformation("Profile updated successfully for user ID: {UserId}", userId);

        return Ok(user);
    }

    /// <summary>
    /// Sync user to customer/barber table (for users created before auto-sync was implemented)
    /// </summary>
    [HttpPost("sync")]
    [Authorize]
    public async Task<ActionResult> SyncUserToCustomerOrBarber()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid token.");
        }

        _logger.LogInformation("Sync request for user ID: {UserId}", userId);

        await _authService.SyncUserToCustomerOrBarberAsync(userId);

        _logger.LogInformation("User synced successfully for user ID: {UserId}", userId);

        return Ok(new { message = "User synced successfully" });
    }

    /// <summary>
    /// Admin creates a new user account (for Barber/Admin accounts)
    /// </summary>
    [HttpPost("create-user")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] RegisterDto registerDto)
    {
        _logger.LogInformation("Admin creating user account for username: {Username}", registerDto.Username);

        var result = await _authService.RegisterAsync(registerDto);

        _logger.LogInformation("User account created successfully by admin: {Username}", registerDto.Username);

        return Ok(result.User);
    }

    /// <summary>
    /// Admin updates user role (with automatic cleanup of old role records)
    /// </summary>
    [HttpPut("update-role/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> UpdateUserRole(int userId, [FromBody] UpdateRoleDto updateRoleDto)
    {
        _logger.LogInformation("Admin updating role for user ID: {UserId} to role: {Role}", userId, updateRoleDto.Role);

        var user = await _authService.UpdateUserRoleAsync(userId, updateRoleDto.Role);

        _logger.LogInformation("User role updated successfully for user ID: {UserId}", userId);

        return Ok(user);
    }
}
