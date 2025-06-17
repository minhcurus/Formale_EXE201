using Application.Interface;
using Application.Service;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStyleController : Controller
    {
        private readonly IProductStyleService _service;

        public ProductStyleController(IProductStyleService service)
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
                return NotFound(new { error = "Style not found" });

            return Ok(new { message = "Found", name });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string styleName)
        {
            var success = await _service.CreateAsync(styleName);
            if (!success)
                return BadRequest(new { error = "Creation failed" });

            return Ok(new { message = "Style created successfully" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateName([FromQuery] Guid styleId, [FromQuery] string styleName)
        {
            var success = await _service.UpdateAsync(styleId, styleName);
            if (!success)
                return NotFound(new { error = "Style not found or update failed" });

            return Ok(new { message = "Style updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteByIdAsync(id);
            if (!success)
                return NotFound(new { error = "Style not found or deletion failed" });

            return Ok(new { message = "Style deleted successfully" });
        }
    }
}
