using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IAuthRepository
{
    Task<UserModel?> GetUserByUsernameAsync(string username);
    Task<UserModel?> GetUserByEmailAsync(string email);
    Task<UserModel?> GetUserByIdAsync(int userId);
    Task<UserModel?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
    Task<IEnumerable<UserModel>> GetAllUsersAsync();
    Task<UserModel> CreateUserAsync(UserModel user);
    Task<UserModel> UpdateUserAsync(UserModel user);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
}
