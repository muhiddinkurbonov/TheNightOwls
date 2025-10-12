using Microsoft.EntityFrameworkCore;
using Fadebook.DB;
using Fadebook.Models;

namespace Fadebook.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly FadebookDbContext _context;

    public AuthRepository(FadebookDbContext context)
    {
        _context = context;
    }

    public async Task<UserModel?> GetUserByUsernameAsync(string username)
    {
        return await _context.userTable
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<UserModel?> GetUserByEmailAsync(string email)
    {
        return await _context.userTable
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<UserModel?> GetUserByIdAsync(int userId)
    {
        return await _context.userTable
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<UserModel?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await _context.userTable
            .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
    }

    public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
    {
        return await _context.userTable
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<UserModel> CreateUserAsync(UserModel user)
    {
        await _context.userTable.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<UserModel> UpdateUserAsync(UserModel user)
    {
        _context.userTable.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.userTable.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.userTable.AnyAsync(u => u.Email == email);
    }
}
