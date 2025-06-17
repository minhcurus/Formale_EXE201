using Application.Interface;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductBrandController : Controller
    {
        private readonly IProductBrandService _service;

        public ProductBrandController(IProductBrandService service)
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
                return NotFound(new { error = "Brand not found" });

            return Ok(new { message = "Found", name });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string brandName)
        {
            var success = await _service.CreateAsync(brandName);
            if (!success)
                return BadRequest(new { error = "Creation failed" });

            return Ok(new { message = "Brand created successfully" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateName([FromQuery] Guid brandId, [FromQuery] string brandName)
        {
            var success = await _service.UpdateAsync(brandId, brandName);
            if (!success)
                return NotFound(new { error = "Brand not found or update failed" });

            return Ok(new { message = "Brand updated successfully" });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteByIdAsync(id);
            if (!success)
                return NotFound(new { error = "Brand not found or deletion failed" });

            return Ok(new { message = "Brand deleted successfully" });
        }
    }
}
