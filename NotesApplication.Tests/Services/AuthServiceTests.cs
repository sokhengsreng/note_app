using Moq;
using NotesApplication.DTOs;
using NotesApplication.Models;
using NotesApplication.Repositories;
using NotesApplication.Services;
using Xunit;

namespace NotesApplication.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockTokenService = new Mock<ITokenService>();
        _authService = new AuthService(_mockUserRepository.Object, _mockPasswordService.Object, _mockTokenService.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var registerDto = new RegisterDto 
        { 
            Username = "testuser", 
            Email = "test@example.com", 
            Password = "password123" 
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.GetByUsernameAsync(registerDto.Username))
            .ReturnsAsync((User?)null);
        _mockPasswordService.Setup(s => s.HashPassword(registerDto.Password))
            .Returns("hashedpassword");
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(1);
        _mockTokenService.Setup(s => s.GenerateToken(1, registerDto.Username, registerDto.Email))
            .Returns("token");

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("User registered successfully", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("testuser", result.Data.Username);
        Assert.Equal("test@example.com", result.Data.Email);
        Assert.Equal("token", result.Data.Token);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnError()
    {
        // Arrange
        var registerDto = new RegisterDto 
        { 
            Username = "testuser", 
            Email = "test@example.com", 
            Password = "password123" 
        };
        var existingUser = new User { Id = 1, Email = "test@example.com" };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Email already registered", result.Message);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var loginDto = new LoginDto 
        { 
            Email = "test@example.com", 
            Password = "password123" 
        };
        var user = new User 
        { 
            Id = 1, 
            Username = "testuser",
            Email = "test@example.com", 
            PasswordHash = "hashedpassword" 
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);
        _mockPasswordService.Setup(s => s.VerifyPassword(loginDto.Password, user.PasswordHash))
            .Returns(true);
        _mockTokenService.Setup(s => s.GenerateToken(user.Id, user.Username, user.Email))
            .Returns("token");

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Login successful", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("token", result.Data.Token);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnError()
    {
        // Arrange
        var loginDto = new LoginDto 
        { 
            Email = "test@example.com", 
            Password = "wrongpassword" 
        };
        var user = new User 
        { 
            Id = 1, 
            Username = "testuser",
            Email = "test@example.com", 
            PasswordHash = "hashedpassword" 
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);
        _mockPasswordService.Setup(s => s.VerifyPassword(loginDto.Password, user.PasswordHash))
            .Returns(false);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid email or password", result.Message);
    }

    [Fact]
    public async Task LoginAsync_WithNonexistentEmail_ShouldReturnError()
    {
        // Arrange
        var loginDto = new LoginDto 
        { 
            Email = "nonexistent@example.com", 
            Password = "password123" 
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid email or password", result.Message);
    }
}
