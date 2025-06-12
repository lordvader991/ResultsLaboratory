using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultsService.Models;
using ResultsService.Services;
using System.Net.Http.Headers;

namespace ResultsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly ResultService _service;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        public ResultsController(ResultService service, IConfiguration config, IHttpClientFactory clientFactory)
        {
            _service = service;
            _config = config;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Result>>> GetAll()
        {
            var results = await _service.GetAllResultsAsync();
            return Ok(results);
        }
        [Authorize]
        [HttpGet("{resultId}")]
        public async Task<ActionResult<Result>> GetResult(int resultId)
        {
            var result = await _service.GetResultAsync(resultId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("orders/{orderId}")]
        public async Task<ActionResult<IEnumerable<Result>>> GetByOrderId(int orderId)
        {
            var results = await _service.GetResultsByOrderIdAsync(orderId);
            if (results == null || !results.Any())
                return NotFound();
            return Ok(results);
        }
        [Authorize]
        [HttpGet("patients/{patientId}")]
        public async Task<ActionResult<IEnumerable<Result>>> GetByPatientId(int patientId)
        {

            var results = await _service.GetResultsByPatientIdAsync(patientId);
            if (results == null || !results.Any())
                return NotFound();
            return Ok(results);
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Result result)
        {
            var httpClient = _clientFactory.CreateClient();

            // Obtener token JWT del request actual
            var accessToken = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(accessToken))
                return Unauthorized("Token no proporcionado");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));

            // Validar paciente
            var patientServiceUrl = _config["PatientServiceUrl"];
            var patientResponse = await httpClient.GetAsync($"{patientServiceUrl}/api/Pacientes/{result.PatientId}");
            if (!patientResponse.IsSuccessStatusCode)
                return BadRequest("El paciente (patientId) no existe.");

            // Validar testType
            var testTypeServiceUrl = _config["TestTypeServiceUrl"];
            var testTypeResponse = await httpClient.GetAsync($"{testTypeServiceUrl}/api/testtypes/{result.TestTypeId}");
            if (!testTypeResponse.IsSuccessStatusCode)
                return BadRequest("El tipo de análisis (testTypeId) no existe.");

            // Obtener la orden y validar orderId + patientId + testTypeId
            var ordersServiceUrl = _config["OrdersServiceUrl"];
            var orderResponse = await httpClient.GetAsync($"{ordersServiceUrl}/orders/{result.OrderId}");
            if (!orderResponse.IsSuccessStatusCode)
                return BadRequest("La orden (orderId) no existe.");

            var orderJson = await orderResponse.Content.ReadAsStringAsync();
            // Usa tu modelo de Order para deserializar
            var order = System.Text.Json.JsonSerializer.Deserialize<OrderDto>(orderJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (order == null)
                return BadRequest("No se pudo obtener la orden.");

            // Verifica que los datos coincidan
            if (order.PatientId != result.PatientId || order.TestTypeId != result.TestTypeId)
                return BadRequest("La orden no corresponde al paciente ni al tipo de análisis enviado.");

            var resultId = await _service.AddResultAsync(result);
            return CreatedAtAction(nameof(GetResult), new { resultId = resultId }, result);
        }

        // Puedes poner tu clase auxiliar aquí mismo o en Models:
        public class OrderDto
        {
            public int OrderId { get; set; }
            public int DoctorId { get; set; }
            public int PatientId { get; set; }
            public int TestTypeId { get; set; }
            public DateTime OrderDate { get; set; }
            public string Status { get; set; }
            public string Notes { get; set; }
        }



        [Authorize]
        [HttpPut("{resultId}")]
        public async Task<ActionResult> Update(int resultId, [FromBody] Result updated)
        {
            var existing = await _service.GetResultAsync(resultId);
            if (existing == null) return NotFound();
            await _service.UpdateResultAsync(resultId, updated);
            return NoContent();
        }
        [Authorize]
        [HttpDelete("{resultId}")]
        public async Task<ActionResult> Delete(int resultId)
        {
            var existing = await _service.GetResultAsync(resultId);
            if (existing == null) return NotFound();
            await _service.DeleteResultAsync(resultId);
            return NoContent();
        }
    }
}
