using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductColorController : Controller
    {
        private readonly IProductColorService _service;

        public ProductColorController(IProductColorService service)
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
                data = data.Select(x => new { Id = x.Item1, Name = x.Item2 })
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNameById(Guid id)
        {
            var name = await _service.GetNameByIdAsync(id);
            if (name == null)
                return NotFound(new { error = "Color not found" });

            return Ok(new { message = "Found", name });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string colorName)
        {
            var success = await _service.CreateAsync(colorName);
            if (!success)
                return BadRequest(new { error = "Creation failed" });

            return Ok(new { message = "Color created successfully" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateName([FromQuery] Guid colorId, [FromQuery] string colorName)
        {
            var success = await _service.UpdateAsync(colorId, colorName);
            if (!success)
                return NotFound(new { error = "Color not found or update failed" });

            return Ok(new { message = "Color updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteByIdAsync(id);
            if (!success)
                return NotFound(new { error = "Color not found or deletion failed" });

            return Ok(new { message = "Color deleted successfully" });
        }
    }
}
