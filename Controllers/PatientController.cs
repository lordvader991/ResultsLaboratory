using Microsoft.AspNetCore.Mvc;
using testAPI_A.Models.DTOS;
using System.Threading.Tasks;

namespace testAPI_A.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly IPatient _service;

        public PacientesController(IPatient service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var paciente = await _service.GetByIdAsync(id);
            if (paciente == null) return NotFound();
            return Ok(paciente);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DTOCreatePatient dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Patient_Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DTOUpdatePatient dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : BadRequest("No se puede eliminar el paciente.");
        }
    }
}
