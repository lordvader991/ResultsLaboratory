using Microsoft.AspNetCore.Mvc;
using FieldsService.Models;
using FieldsService.Services;

namespace FieldsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FieldsController : ControllerBase
    {
        private readonly FieldService _service;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        public FieldsController(FieldService service, IConfiguration config, IHttpClientFactory clientFactory)
        {
            _service = service;
            _config = config;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Field>>> GetAll()
        {
            var fields = await _service.GetAllFieldsAsync();
            return Ok(fields);
        }

        [HttpGet("{testTypeId}")]
        public async Task<ActionResult<IEnumerable<Field>>> GetByTestType(int testTypeId)
        {
            var fields = await _service.GetFieldsByTestTypeAsync(testTypeId);
            return Ok(fields);
        }

        [HttpGet("{testTypeId}/{fieldId}")]
        public async Task<ActionResult<Field>> GetField(int testTypeId, int fieldId)
        {
            var field = await _service.GetFieldAsync(testTypeId, fieldId);
            if (field == null) return NotFound();
            return Ok(field);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Field field)
        {
            // Validar existencia de testTypeId en TestTypeService
            var httpClient = _clientFactory.CreateClient();
            var testTypeServiceUrl = _config["TestTypeServiceUrl"];
            var response = await httpClient.GetAsync($"{testTypeServiceUrl}/api/testtypes/{field.TestTypeId}");
            if (!response.IsSuccessStatusCode)
                return BadRequest("El tipo de análisis (testTypeId) no existe.");

            var fieldId = await _service.AddFieldAsync(field);
            return CreatedAtAction(nameof(GetField), new { testTypeId = field.TestTypeId, fieldId = fieldId }, field);
        }


    }
}
