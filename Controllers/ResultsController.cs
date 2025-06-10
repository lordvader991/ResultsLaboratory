using Microsoft.AspNetCore.Mvc;
using ResultsService.Models;
using ResultsService.Services;

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

        [HttpGet("{resultId}")]
        public async Task<ActionResult<Result>> GetResult(int resultId)
        {
            var result = await _service.GetResultAsync(resultId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("orders/{orderId}")]
        public async Task<ActionResult<IEnumerable<Result>>> GetByOrderId(int orderId)
        {
            var results = await _service.GetResultsByOrderIdAsync(orderId);
            if (results == null || !results.Any())
                return NotFound();
            return Ok(results);
        }

        [HttpGet("patients/{patientId}")]
        public async Task<ActionResult<IEnumerable<Result>>> GetByPatientId(int patientId)
        {
            var results = await _service.GetResultsByPatientIdAsync(patientId);
            if (results == null || !results.Any())
                return NotFound();
            return Ok(results);
        }



        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Result result)
        {
            var httpClient = _clientFactory.CreateClient();

            // Validación de Patient
            var patientServiceUrl = _config["PatientServiceUrl"];
            var patientResponse = await httpClient.GetAsync($"{patientServiceUrl}/api/Pacientes/{result.PatientId}");
            if (!patientResponse.IsSuccessStatusCode)
                return BadRequest("El paciente (patientId) no existe.");

            // Validación de testType
            var testTypeServiceUrl = _config["TestTypeServiceUrl"];
            var testTypeResponse = await httpClient.GetAsync($"{testTypeServiceUrl}/api/testtypes/{result.TestTypeId}");
            if (!testTypeResponse.IsSuccessStatusCode)
                return BadRequest("El tipo de análisis (testTypeId) no existe.");

            // Validación de Order
            var ordersServiceUrl = _config["OrdersServiceUrl"];
            var orderResponse = await httpClient.GetAsync($"{ordersServiceUrl}/orders/{result.OrderId}");
            if (!orderResponse.IsSuccessStatusCode)
                return BadRequest("La orden (orderId) no existe.");

            var resultId = await _service.AddResultAsync(result);
            return CreatedAtAction(nameof(GetResult), new { resultId = resultId }, result);
        }



        [HttpPut("{resultId}")]
        public async Task<ActionResult> Update(int resultId, [FromBody] Result updated)
        {
            var existing = await _service.GetResultAsync(resultId);
            if (existing == null) return NotFound();
            await _service.UpdateResultAsync(resultId, updated);
            return NoContent();
        }

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
