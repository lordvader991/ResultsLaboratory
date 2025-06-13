using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CassandraJwtAuth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProtectedController : ControllerBase
{
    [Authorize]
    [HttpGet("secret")]
    public IActionResult Secret()
    {;
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
