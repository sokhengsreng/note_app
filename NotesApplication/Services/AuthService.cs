using NotesApplication.DTOs;
using NotesApplication.Models;
using NotesApplication.Repositories;

namespace NotesApplication.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IPasswordService passwordService, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Username, email, and password are required",
                Errors = new List<string> { "Username, email, and password are required" }
            };
        }

        // Check if user already exists
        var existingUserByEmail = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUserByEmail != null)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Email already registered",
                Errors = new List<string> { "Email already registered" }
            };
        }

        var existingUserByUsername = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existingUserByUsername != null)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Username already taken",
                Errors = new List<string> { "Username already taken" }
            };
        }

        // Create new user
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = _passwordService.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        var userId = await _userRepository.AddAsync(user);
        var token = _tokenService.GenerateToken(userId, user.Username, user.Email);

        return new ApiResponse<AuthResponseDto>
        {
            Success = true,
            Message = "User registered successfully",
            Data = new AuthResponseDto
            {
                UserId = userId,
                Username = user.Username,
                Email = user.Email,
                Token = token
            }
        };
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Email and password are required",
                Errors = new List<string> { "Email and password are required" }
            };
        }

        // Find user by email
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Invalid email or password",
                Errors = new List<string> { "Invalid email or password" }
            };
        }

        // Verify password
        if (!_passwordService.VerifyPassword(dto.Password, user.PasswordHash))
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Invalid email or password",
                Errors = new List<string> { "Invalid email or password" }
            };
        }

        var token = _tokenService.GenerateToken(user.Id, user.Username, user.Email);

        return new ApiResponse<AuthResponseDto>
        {
            Success = true,
            Message = "Login successful",
            Data = new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token
            }
        };
    }
}
