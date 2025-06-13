using Microsoft.AspNetCore.Mvc;
using CassandraJwtAuth.Services;
using CassandraJwtAuth.Models;
using Microsoft.AspNetCore.Authorization;

namespace CassandraJwtAuth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtService _jwtService;

    public AuthController(AuthService authService, JwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User login)
    {
        if (string.IsNullOrEmpty(login?.Email))
        {
            return BadRequest("El correo electrónico es requerido.");
        }

        var user = await _authService.GetUserByEmailAsync(login.Email);
        if (user == null || string.IsNullOrEmpty(login?.PasswordHash) || string.IsNullOrEmpty(user.PasswordHash) || !_authService.VerifyPassword(login.PasswordHash, user.PasswordHash))
        {
            return Unauthorized("Correo o contraseña inválidos");
        }

        if (string.IsNullOrEmpty(user.Email))
        {
            return StatusCode(500, "El usuario no tiene un correo electrónico válido.");
        }

        // Nueva línea: genera token con ID y email
        var token = _jwtService.GenerateToken(user.Id, user.Email);

        return Ok(new { token });
    }

    [Authorize]
    [HttpGet("status")]
    public IActionResult Secret()
    {
        ;
        var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.Identity?.Name ?? "desconocido";
        return Ok(new
        {
            Message = "Ruta protegida accedida",
            UserId = id,
            Email = email
        });
    }
}
