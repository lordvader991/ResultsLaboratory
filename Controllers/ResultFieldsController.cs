using Microsoft.AspNetCore.Mvc;
using ResultFieldsService.Models;
using ResultFieldsService.Services;

namespace ResultFieldsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultFieldsController : ControllerBase
    {
        private readonly ResultFieldService _service;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        public ResultFieldsController(ResultFieldService service, IConfiguration config, IHttpClientFactory clientFactory)
        {
            _service = service;
            _config = config;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultField>>> GetAll()
        {
            var resultFields = await _service.GetAllAsync();
            return Ok(resultFields);
        }

        [HttpGet("{resultFieldId}")]
        public async Task<ActionResult<ResultField>> GetById(int resultFieldId)
        {
            var rf = await _service.GetByIdAsync(resultFieldId);
            if (rf == null) return NotFound();
            return Ok(rf);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ResultField rf)
        {
            var httpClient = _clientFactory.CreateClient();
            // Validar existencia de testTypeId en TestTypeService
            var testTypeServiceUrl = _config["TestTypeServiceUrl"];
            var responseType = await httpClient.GetAsync($"{testTypeServiceUrl}/api/testtypes/{rf.TestTypeId}");
            if (!responseType.IsSuccessStatusCode)
                return BadRequest("El tipo de análisis (testTypeId) no existe.");

            // Validar existencia de ResultId en ResultsService
            var resultsServiceUrl = _config["ResultsServiceUrl"];

            // Obtener token JWT del request actual
            var accessToken = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(accessToken))
                return Unauthorized("Token no proporcionado");

            // Agregar el token al header de la petición
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));

            var respResult = await httpClient.GetAsync($"{resultsServiceUrl}/api/results/{rf.ResultId}");
            if (!respResult.IsSuccessStatusCode)
                return BadRequest("El resultado (resultId) no existe.");

            // Validar existencia de Field en FieldsService
            var fieldsServiceUrl = _config["FieldsServiceUrl"];
            var respField = await httpClient.GetAsync($"{fieldsServiceUrl}/api/fields/{rf.TestTypeId}/{rf.FieldId}");
            if (!respField.IsSuccessStatusCode)
                return BadRequest("El campo (testTypeId/fieldId) no existe.");

            var resultFieldId = await _service.AddAsync(rf);
            return CreatedAtAction(nameof(GetById), new { resultFieldId = resultFieldId }, rf);
        }



        [HttpPut("{resultFieldId}")]
        public async Task<ActionResult> Update(int resultFieldId, [FromBody] ResultField updated)
        {
            var existing = await _service.GetByIdAsync(resultFieldId);
            if (existing == null) return NotFound();
            await _service.UpdateAsync(resultFieldId, updated);
            return NoContent();
        }

        [HttpDelete("{resultFieldId}")]
        public async Task<ActionResult> Delete(int resultFieldId)
        {
            var existing = await _service.GetByIdAsync(resultFieldId);
            if (existing == null) return NotFound();
            await _service.DeleteAsync(resultFieldId);
            return NoContent();
        }
    }
}
