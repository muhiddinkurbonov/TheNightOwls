using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using Fadebook.DTO;
using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.Exceptions;

namespace Fadebook.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IBarberRepository _barberRepository;
    private readonly IDbTransactionContext _dbTransactionContext;
    private readonly IMapper _mapper;
    private readonly string _jwtSecretKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtExpirationMinutes;

    public AuthService(
        IAuthRepository authRepository,
        ICustomerRepository customerRepository,
        IBarberRepository barberRepository,
        IDbTransactionContext dbTransactionContext,
        IMapper mapper)
    {
        _authRepository = authRepository;
        _customerRepository = customerRepository;
        _barberRepository = barberRepository;
        _dbTransactionContext = dbTransactionContext;
        _mapper = mapper;

        // Load JWT settings from environment variables
        _jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
            ?? throw new InvalidOperationException("JWT_SECRET_KEY is not configured in .env");
        _jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            ?? throw new InvalidOperationException("JWT_ISSUER is not configured in .env");
        _jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
            ?? throw new InvalidOperationException("JWT_AUDIENCE is not configured in .env");

        var expirationStr = Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES");
        _jwtExpirationMinutes = int.TryParse(expirationStr, out var exp) ? exp : 60;
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if username already exists
        if (await _authRepository.UsernameExistsAsync(registerDto.Username))
        {
            throw new BadRequestException($"Username '{registerDto.Username}' is already taken.");
        }

        // Check if email already exists
        if (await _authRepository.EmailExistsAsync(registerDto.Email))
        {
            throw new BadRequestException($"Email '{registerDto.Email}' is already registered.");
        }

        // Hash the password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // Create user model
        var user = new UserModel
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            Name = registerDto.Name,
            PhoneNumber = registerDto.PhoneNumber,
            Role = registerDto.Role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Save to database
        var createdUser = await _authRepository.CreateUserAsync(user);
        await _dbTransactionContext.SaveChangesAsync();

        // Auto-create Customer or Barber record based on role
        try
        {
            await CreateRoleBasedRecordAsync(createdUser, checkExisting: false);
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the registration
            // The user account was created successfully
            Console.WriteLine($"Warning: Failed to create {createdUser.Role} record for user {createdUser.Username}: {ex.Message}");
        }

        // Generate JWT token
        var token = GenerateJwtToken(createdUser);

        // Map to DTO and return
        var userDto = _mapper.Map<UserDto>(createdUser);

        // Populate role-specific IDs
        await PopulateRoleSpecificIds(userDto);

        return new LoginResponseDto
        {
            Token = token,
            User = userDto
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Find user by username or email
        var user = await _authRepository.GetUserByUsernameOrEmailAsync(loginDto.UsernameOrEmail);

        if (user == null)
        {
            throw new UnauthorizedException("Invalid username/email or password.");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid username/email or password.");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            throw new UnauthorizedException("Your account has been deactivated.");
        }

        // Update last login time
        user.LastLoginAt = DateTime.UtcNow;
        await _authRepository.UpdateUserAsync(user);

        // Generate JWT token
        var token = GenerateJwtToken(user);

        // Map to DTO and return
        var userDto = _mapper.Map<UserDto>(user);

        // Populate role-specific IDs
        await PopulateRoleSpecificIds(userDto);

        return new LoginResponseDto
        {
            Token = token,
            User = userDto
        };
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        var user = await _authRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _authRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedException("Current password is incorrect.");
        }

        // Hash new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

        // Update user
        var updatedUser = await _authRepository.UpdateUserAsync(user);

        return _mapper.Map<UserDto>(updatedUser);
    }

    public async Task<UserDto> UpdateUserAsync(int userId, UserDto userDto)
    {
        var user = await _authRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }

        // Update allowed fields
        user.Name = userDto.Name;
        user.PhoneNumber = userDto.PhoneNumber;

        // Update user
        var updatedUser = await _authRepository.UpdateUserAsync(user);

        return _mapper.Map<UserDto>(updatedUser);
    }

    public async Task SyncUserToCustomerOrBarberAsync(int userId)
    {
        var user = await _authRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }

        await CreateRoleBasedRecordAsync(user, checkExisting: true);
    }

    private async Task CreateRoleBasedRecordAsync(UserModel user, bool checkExisting = false)
    {
        if (user.Role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
        {
            if (checkExisting)
            {
                var existingCustomer = await _customerRepository.GetByUsernameAsync(user.Username);
                if (existingCustomer != null) return;
            }

            var customer = new CustomerModel
            {
                Username = user.Username,
                Name = user.Name,
                ContactInfo = user.PhoneNumber ?? user.Email
            };
            await _customerRepository.AddAsync(customer);
            await _dbTransactionContext.SaveChangesAsync();
        }
        else if (user.Role.Equals("Barber", StringComparison.OrdinalIgnoreCase))
        {
            if (checkExisting)
            {
                // Check by username for consistency
                var existingBarber = await _barberRepository.GetAllAsync();
                if (existingBarber.Any(b => b.Username == user.Username)) return;
            }

            var barber = new BarberModel
            {
                Username = user.Username,
                Name = user.Name,
                Specialty = "General",
                ContactInfo = user.PhoneNumber ?? user.Email
            };
            await _barberRepository.AddAsync(barber);
            await _dbTransactionContext.SaveChangesAsync();
        }
    }

    private async Task PopulateRoleSpecificIds(UserDto userDto)
    {
        if (userDto.Role.Equals("Barber", StringComparison.OrdinalIgnoreCase))
        {
            var barbers = await _barberRepository.GetAllAsync();
            var barber = barbers.FirstOrDefault(b => b.Username == userDto.Username);
            if (barber != null)
            {
                userDto.BarberId = barber.BarberId;
            }
        }
        else if (userDto.Role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
        {
            var customer = await _customerRepository.GetByUsernameAsync(userDto.Username);
            if (customer != null)
            {
                userDto.CustomerId = customer.CustomerId;
            }
        }
    }

    private string GenerateJwtToken(UserModel user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
