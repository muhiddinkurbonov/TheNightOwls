using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.Services;
using Fadebook.DTO;
using Fadebook.Exceptions;

namespace Api.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly Mock<IAuthRepository> _mockAuthRepository;
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<IBarberRepository> _mockBarberRepository;
    private readonly Mock<IDbTransactionContext> _mockDbTransactionContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        // Set up environment variables for JWT configuration
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "TestSecretKeyForUnitTests123456789012345678901234567890");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "TestIssuer");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "TestAudience");
        Environment.SetEnvironmentVariable("JWT_EXPIRATION_MINUTES", "60");

        _mockAuthRepository = new Mock<IAuthRepository>();
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockBarberRepository = new Mock<IBarberRepository>();
        _mockDbTransactionContext = new Mock<IDbTransactionContext>();
        _mockMapper = new Mock<IMapper>();

        _service = new AuthService(
            _mockAuthRepository.Object,
            _mockCustomerRepository.Object,
            _mockBarberRepository.Object,
            _mockDbTransactionContext.Object,
            _mockMapper.Object
        );
    }

    public void Dispose()
    {
        // Clean up environment variables
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", null);
        Environment.SetEnvironmentVariable("JWT_ISSUER", null);
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", null);
        Environment.SetEnvironmentVariable("JWT_EXPIRATION_MINUTES", null);
    }

    [Fact]
    public async Task RegisterAsync_WhenValidCustomer_ShouldCreateUserAndReturnToken()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "Password123!",
            Name = "New User",
            PhoneNumber = "1234567890",
            Role = "Customer"
        };
        var createdUser = new UserModel
        {
            UserId = 1,
            Username = "newuser",
            Email = "newuser@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Name = "New User",
            PhoneNumber = "1234567890",
            Role = "Customer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        var userDto = new UserDto
        {
            UserId = 1,
            Username = "newuser",
            Email = "newuser@example.com",
            Name = "New User",
            PhoneNumber = "1234567890",
            Role = "Customer"
        };

        _mockAuthRepository.Setup(r => r.UsernameExistsAsync("newuser")).ReturnsAsync(false);
        _mockAuthRepository.Setup(r => r.EmailExistsAsync("newuser@example.com")).ReturnsAsync(false);
        _mockAuthRepository.Setup(r => r.CreateUserAsync(It.IsAny<UserModel>())).ReturnsAsync(createdUser);
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockCustomerRepository.Setup(r => r.GetByUsernameAsync("newuser")).ReturnsAsync((CustomerModel?)null);
        _mockCustomerRepository.Setup(r => r.AddAsync(It.IsAny<CustomerModel>())).ReturnsAsync(new CustomerModel());
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<UserModel>())).Returns(userDto);

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Username.Should().Be("newuser");
        _mockAuthRepository.Verify(r => r.CreateUserAsync(It.IsAny<UserModel>()), Times.Once);
        _mockDbTransactionContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RegisterAsync_WhenUsernameExists_ShouldThrowBadRequestException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "existinguser",
            Email = "newuser@example.com",
            Password = "Password123!",
            Name = "New User",
            Role = "Customer"
        };

        _mockAuthRepository.Setup(r => r.UsernameExistsAsync("existinguser")).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.RegisterAsync(registerDto));
        _mockAuthRepository.Verify(r => r.CreateUserAsync(It.IsAny<UserModel>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ShouldThrowBadRequestException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "existing@example.com",
            Password = "Password123!",
            Name = "New User",
            Role = "Customer"
        };

        _mockAuthRepository.Setup(r => r.UsernameExistsAsync("newuser")).ReturnsAsync(false);
        _mockAuthRepository.Setup(r => r.EmailExistsAsync("existing@example.com")).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.RegisterAsync(registerDto));
        _mockAuthRepository.Verify(r => r.CreateUserAsync(It.IsAny<UserModel>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = "Password123!"
        };
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");
        var user = new UserModel
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            PasswordHash = passwordHash,
            Name = "Test User",
            Role = "Customer",
            IsActive = true
        };
        var userDto = new UserDto
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            Name = "Test User",
            Role = "Customer"
        };

        _mockAuthRepository.Setup(r => r.GetUserByUsernameOrEmailAsync("testuser")).ReturnsAsync(user);
        _mockAuthRepository.Setup(r => r.UpdateUserAsync(It.IsAny<UserModel>())).ReturnsAsync(user);
        _mockCustomerRepository.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync((CustomerModel?)null);
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<UserModel>())).Returns(userDto);

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Username.Should().Be("testuser");
        _mockAuthRepository.Verify(r => r.UpdateUserAsync(It.IsAny<UserModel>()), Times.Once); // LastLoginAt update
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "nonexistent",
            Password = "Password123!"
        };

        _mockAuthRepository.Setup(r => r.GetUserByUsernameOrEmailAsync("nonexistent")).ReturnsAsync((UserModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_WhenInvalidPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = "WrongPassword!"
        };
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");
        var user = new UserModel
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            PasswordHash = passwordHash,
            Name = "Test User",
            Role = "Customer",
            IsActive = true
        };

        _mockAuthRepository.Setup(r => r.GetUserByUsernameOrEmailAsync("testuser")).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_WhenUserInactive_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "testuser",
            Password = "Password123!"
        };
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");
        var user = new UserModel
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            PasswordHash = passwordHash,
            Name = "Test User",
            Role = "Customer",
            IsActive = false // Inactive user
        };

        _mockAuthRepository.Setup(r => r.GetUserByUsernameOrEmailAsync("testuser")).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.LoginAsync(loginDto));
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUserDto()
    {
        // Arrange
        var user = new UserModel
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            Name = "Test User",
            Role = "Customer"
        };
        var userDto = new UserDto
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            Name = "Test User",
            Role = "Customer"
        };

        _mockAuthRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.GetUserByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(1);
        result.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockAuthRepository.Setup(r => r.GetUserByIdAsync(999)).ReturnsAsync((UserModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetUserByIdAsync(999));
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenValidCurrentPassword_ShouldUpdatePassword()
    {
        // Arrange
        var changePasswordDto = new ChangePasswordDto
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!"
        };
        var currentPasswordHash = BCrypt.Net.BCrypt.HashPassword("OldPassword123!");
        var user = new UserModel
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            PasswordHash = currentPasswordHash,
            Name = "Test User",
            Role = "Customer"
        };
        var userDto = new UserDto
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            Name = "Test User",
            Role = "Customer"
        };

        _mockAuthRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);
        _mockAuthRepository.Setup(r => r.UpdateUserAsync(It.IsAny<UserModel>())).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<UserModel>())).Returns(userDto);

        // Act
        var result = await _service.ChangePasswordAsync(1, changePasswordDto);

        // Assert
        result.Should().NotBeNull();
        _mockAuthRepository.Verify(r => r.UpdateUserAsync(It.IsAny<UserModel>()), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenInvalidCurrentPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var changePasswordDto = new ChangePasswordDto
        {
            CurrentPassword = "WrongPassword!",
            NewPassword = "NewPassword123!"
        };
        var currentPasswordHash = BCrypt.Net.BCrypt.HashPassword("OldPassword123!");
        var user = new UserModel
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            PasswordHash = currentPasswordHash,
            Name = "Test User",
            Role = "Customer"
        };

        _mockAuthRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.ChangePasswordAsync(1, changePasswordDto));
        _mockAuthRepository.Verify(r => r.UpdateUserAsync(It.IsAny<UserModel>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenUserExists_ShouldUpdateAndReturn()
    {
        // Arrange
        var userDto = new UserDto
        {
            Name = "Updated Name",
            PhoneNumber = "9876543210"
        };
        var user = new UserModel
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            Name = "Old Name",
            PhoneNumber = "1234567890",
            Role = "Customer"
        };
        var updatedUserDto = new UserDto
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            Name = "Updated Name",
            PhoneNumber = "9876543210",
            Role = "Customer"
        };

        _mockAuthRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);
        _mockAuthRepository.Setup(r => r.UpdateUserAsync(It.IsAny<UserModel>())).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<UserModel>())).Returns(updatedUserDto);

        // Act
        var result = await _service.UpdateUserAsync(1, userDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
        result.PhoneNumber.Should().Be("9876543210");
        _mockAuthRepository.Verify(r => r.UpdateUserAsync(It.Is<UserModel>(u =>
            u.Name == "Updated Name" && u.PhoneNumber == "9876543210")), Times.Once);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_WhenChangingToBarber_ShouldUpdateRoleAndCreateBarberRecord()
    {
        // Arrange
        var user = new UserModel
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            Name = "Test User",
            Role = "Customer"
        };
        var customer = new CustomerModel
        {
            CustomerId = 1,
            Username = "testuser",
            Name = "Test User"
        };
        var userDto = new UserDto
        {
            UserId = 1,
            Username = "testuser",
            Role = "Barber"
        };

        _mockAuthRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);
        _mockCustomerRepository.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync(customer);
        _mockCustomerRepository.Setup(r => r.RemoveByIdAsync(1)).ReturnsAsync(customer);
        _mockAuthRepository.Setup(r => r.UpdateUserAsync(It.IsAny<UserModel>())).ReturnsAsync(user);
        _mockBarberRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<BarberModel>());
        _mockBarberRepository.Setup(r => r.AddAsync(It.IsAny<BarberModel>())).ReturnsAsync(new BarberModel());
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<UserModel>())).Returns(userDto);

        // Act
        var result = await _service.UpdateUserRoleAsync(1, "Barber");

        // Assert
        result.Should().NotBeNull();
        result.Role.Should().Be("Barber");
        _mockCustomerRepository.Verify(r => r.RemoveByIdAsync(1), Times.Once); // Cleanup old role
        _mockBarberRepository.Verify(r => r.AddAsync(It.IsAny<BarberModel>()), Times.Once); // Create new role
    }

    [Fact]
    public async Task UpdateUserRoleAsync_WhenInvalidRole_ShouldThrowBadRequestException()
    {
        // Arrange
        var user = new UserModel
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            Name = "Test User",
            Role = "Customer"
        };

        _mockAuthRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateUserRoleAsync(1, "InvalidRole"));
    }

    [Fact]
    public async Task UpdateUserRoleAsync_WhenRoleUnchanged_ShouldReturnUserWithoutUpdating()
    {
        // Arrange
        var user = new UserModel
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            Name = "Test User",
            Role = "Customer"
        };
        var userDto = new UserDto
        {
            UserId = 1,
            Username = "testuser",
            Role = "Customer"
        };

        _mockAuthRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<UserModel>())).Returns(userDto);

        // Act
        var result = await _service.UpdateUserRoleAsync(1, "Customer");

        // Assert
        result.Should().NotBeNull();
        result.Role.Should().Be("Customer");
        _mockCustomerRepository.Verify(r => r.RemoveByIdAsync(It.IsAny<int>()), Times.Never);
        _mockBarberRepository.Verify(r => r.AddAsync(It.IsAny<BarberModel>()), Times.Never);
    }

    [Fact]
    public async Task SyncUserToCustomerOrBarberAsync_WhenCustomerDoesNotExist_ShouldCreateCustomer()
    {
        // Arrange
        var user = new UserModel
        {
            UserId = 1,
            Username = "testuser",
            Email = "testuser@example.com",
            Name = "Test User",
            PhoneNumber = "1234567890",
            Role = "Customer"
        };

        _mockAuthRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);
        _mockCustomerRepository.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync((CustomerModel?)null);
        _mockCustomerRepository.Setup(r => r.AddAsync(It.IsAny<CustomerModel>())).ReturnsAsync(new CustomerModel());
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _service.SyncUserToCustomerOrBarberAsync(1);

        // Assert
        _mockCustomerRepository.Verify(r => r.AddAsync(It.Is<CustomerModel>(c =>
            c.Username == "testuser" && c.Name == "Test User")), Times.Once);
    }

    [Fact]
    public async Task SyncUserToCustomerOrBarberAsync_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockAuthRepository.Setup(r => r.GetUserByIdAsync(999)).ReturnsAsync((UserModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.SyncUserToCustomerOrBarberAsync(999));
    }
}
