using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using CassandraJwtAuth.Models;
using System.Threading.Tasks;
using CassandraJwtAuth.Services;

namespace CassandraJwtAuth.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly AuthService _authService;

    public UserController(AuthService authService)
    {
        _authService = authService;
    }

    [Authorize]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await _authService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound("Usuario no encontrado");
        }
        return Ok(
            new
            {
                user.Id,
                user.Email,
                user.Name,
                user.Specialty,
                user.Address,
                user.LicenseNumber
            }
        );
    }
}
