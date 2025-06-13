using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using CassandraJwtAuth.Models;

namespace CassandraJwtAuth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{

    [Authorize]
    [HttpGet("list")]
    public IActionResult GetOrders()
    {
        var doctorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        // Aquí podrías obtener los pedidos del usuario autenticado
        return Ok(new 
        {
            Message = "Lista de todas las órdenes",
            doctor_id = doctorId
        });
    }

    [Authorize]
    [HttpPost("create")]

    public async Task<IActionResult> CreateOrder([FromBody] Order newOrder)
    {
        var doctorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        using var httpClient = new HttpClient();

        var url = "http://localhost:3002/api/orders/create";

        // Crear un objeto anónimo que contenga doctor_id y el resto de los datos de la orden
        var payload = new
        {
            doctor_id = doctorId,
            order = newOrder
        };

        // Serializar el objeto completo a JSON
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        return Content(responseContent, "application/json");
    }
}