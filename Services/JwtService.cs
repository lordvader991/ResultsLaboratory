using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CassandraJwtAuth.Services;

public class JwtService
{
    private readonly string? _secret;
    public JwtService(IConfiguration config)
    {
        _secret = config["Jwt:Key"];
    }

    public string GenerateToken(int userId, string email)
{
    var tokenHandler = new JwtSecurityTokenHandler();

    if (string.IsNullOrEmpty(_secret))
        throw new InvalidOperationException("JWT secret key is not configured.");

    var key = Encoding.UTF8.GetBytes(_secret);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // ID del usuario
            new Claim(ClaimTypes.Email, email) // Email del usuario
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        )
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

}
