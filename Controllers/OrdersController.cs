using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly CassandraOrderRepository _repository;

        public OrdersController(CassandraOrderRepository repository)
        {
            _repository = repository;
        }
        [Authorize]
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetOrdersByDoctor(int doctorId)
        {
            var orders = await _repository.GetOrdersByDoctorAsync(doctorId);
            return Ok(orders);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (order == null)
                return BadRequest("La orden no puede ser nula.");

            try
            {
                if (order.OrderId == 0) order.OrderId = new Random().Next(1, int.MaxValue);
                if (order.OrderDate == default) order.OrderDate = DateTime.UtcNow;

                await _repository.InsertOrderAsync(order);
                Console.WriteLine($"Orden insertada vía API: {order.OrderId}");
                return CreatedAtAction(nameof(GetOrdersByDoctor), new { doctorId = order.DoctorId }, order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar orden vía API: {ex.Message}");
                return StatusCode(500, "Error interno al guardar la orden.");
            }
        }
        [Authorize]
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var orders = await _repository.GetOrdersByPatientAsync(patientId);
            return Ok(orders);
        }
        [Authorize]
        [HttpGet("test-type/{testTypeId}")]
        public async Task<IActionResult> GetByTestType(int testTypeId)
        {
            var orders = await _repository.GetOrdersByTestTypeAsync(testTypeId);
            return Ok(orders);
        }
        [Authorize]
        [HttpGet("patient/{patientId}/type/{testTypeId}")]
        public async Task<IActionResult> GetByPatientAndType(int patientId, int testTypeId)
        {
            var orders = await _repository.GetOrdersByPatientAndTypeAsync(patientId, testTypeId);
            return Ok(orders);
        }
        [Authorize]
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderByIdWithFiltering(int orderId)
        {
            if (orderId == 0) return BadRequest("Se requiere orderId.");
            var order = await _repository.GetOrderByIdWithFilteringAsync(orderId);
            if (order == null) return NotFound("Orden no encontrada.");
            return Ok(order);
        }


        [Authorize]
        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(
            int orderId,
            [FromQuery] int doctorId,
            [FromBody] OrderUpdateDto updateDto)
        {
            if (orderId == 0 || doctorId == 0 || updateDto == null)
                return BadRequest("Datos inválidos.");

            var order = await _repository.GetOrderByIdAsync(orderId, doctorId);
            if (order == null)
                return NotFound("Orden no encontrada.");

            await _repository.UpdateOrderAsync(
                orderId,
                order.DoctorId,
                order.PatientId,
                order.TestTypeId,
                updateDto.Status,
                updateDto.Notes
            );

            return Ok("Orden actualizada correctamente.");
        }
        [Authorize]
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId, [FromQuery] int doctorId)
        {
            if (orderId == 0 || doctorId == 0)
                return BadRequest("Se requiere el orderId y doctorId.");

            var order = await _repository.GetOrderByIdAsync(orderId, doctorId);
            if (order == null)
                return NotFound("La orden no existe.");

            try
            {
                await _repository.DeleteOrderAsync(order);
                Console.WriteLine($"Orden eliminada: {orderId}");
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar orden: {ex.Message}");
                return StatusCode(500, "Error interno eliminando la orden.");
            }
        }
    }
}
