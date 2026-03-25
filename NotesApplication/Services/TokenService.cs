using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NotesApplication.Services;

public interface ITokenService
{
    string GenerateToken(int userId, string username, string email);
}

public class TokenService : ITokenService
{
    private readonly string _secretKey;
    private readonly int _expirationMinutes;

    public TokenService(IConfiguration configuration)
    {
        _secretKey = configuration["JwtSettings:SecretKey"] ?? "your-super-secret-key-change-in-production";
        _expirationMinutes = int.TryParse(configuration["JwtSettings:ExpirationMinutes"], out var minutes) ? minutes : 60;
    }

    public string GenerateToken(int userId, string username, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email)
        };

        var token = new JwtSecurityToken(
            issuer: "NotesApplication",
            audience: "NotesApplicationUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
