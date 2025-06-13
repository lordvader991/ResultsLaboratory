using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using CassandraJwtAuth.Models;

namespace CassandraJwtAuth.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.Identity?.Name ?? "desconocido";

        // Aquí podrías obtener más información del usuario desde la base de datos si es necesario

        return Ok(new
        {
            Message = "Perfil de usuario accedido",
            UserId = userId,
            Email = email
        });
    }
}