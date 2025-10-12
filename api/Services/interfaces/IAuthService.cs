using Fadebook.DTO;

namespace Fadebook.Services;

public interface IAuthService
{
    Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    Task<UserDto> GetUserByIdAsync(int userId);
    Task<UserDto> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<UserDto> UpdateUserAsync(int userId, UserDto userDto);
    Task SyncUserToCustomerOrBarberAsync(int userId);
}
