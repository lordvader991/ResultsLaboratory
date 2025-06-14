using Microsoft.AspNetCore.Mvc;
using TestTypeService.Models;

namespace TestTypeService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestTypesController : ControllerBase
    {
        private readonly TestTypeService.Services.TestTypeService _service;

        public TestTypesController(TestTypeService.Services.TestTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestType>>> GetAll()
        {
            var testTypes = await _service.GetAllAsync();
            return Ok(testTypes);
        }

        [HttpGet("{testTypeId}")]
        public async Task<ActionResult<TestType>> GetById(int testTypeId)
        {
            var testType = await _service.GetByIdAsync(testTypeId);
            if (testType == null) return NotFound();
            return Ok(testType);
        }



    }
}
