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

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Result result)
        {
            // Validar existencia de testTypeId en TestTypeService
            var httpClient = _clientFactory.CreateClient();
            var testTypeServiceUrl = _config["TestTypeServiceUrl"];
            var response = await httpClient.GetAsync($"{testTypeServiceUrl}/api/testtypes/{result.TestTypeId}");
            if (!response.IsSuccessStatusCode)
                return BadRequest("El tipo de análisis (testTypeId) no existe.");

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
