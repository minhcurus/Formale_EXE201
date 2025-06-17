using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductMaterialController : Controller
    {
        private readonly IProductMaterialService _service;

        public ProductMaterialController(IProductMaterialService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(new
            {
                message = "Retrieved successfully",
                data = data.Select(x => new { x.MaterialId, x.MaterialName })
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNameById(Guid id)
        {
            var name = await _service.GetNameByIdAsync(id);
            if (name == null)
                return NotFound(new { error = "Material not found" });

            return Ok(new { message = "Found", name });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string materialName)
        {
            var success = await _service.CreateAsync(materialName);
            if (!success)
                return BadRequest(new { error = "Creation failed" });

            return Ok(new { message = "Material created successfully" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateName([FromQuery] Guid materialId, [FromQuery] string materialName)
        {
            var success = await _service.UpdateAsync(materialId, materialName);
            if (!success)
                return NotFound(new { error = "Material not found or update failed" });

            return Ok(new { message = "Material updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteByIdAsync(id);
            if (!success)
                return NotFound(new { error = "Category not found or deletion failed" });

            return Ok(new { message = "Category deleted successfully" });
        }
    }
}
