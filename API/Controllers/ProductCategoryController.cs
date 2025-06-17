using Application.DTO;
using Application.Interface;
using Application.Service;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductCategoryController : Controller
    {
        private readonly IProductCategoryService _service;

        public ProductCategoryController(IProductCategoryService service)
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
                data = data.Select(x => new { x.CategoryId, x.CategoryName })
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNameById(Guid id)
        {
            var name = await _service.GetNameByIdAsync(id);
            if (name == null)
                return NotFound(new { error = "Category not found" });

            return Ok(new { message = "Found", name });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string categoryName)
        {
            var success = await _service.CreateAsync(categoryName);
            if (!success)
                return BadRequest(new { error = "Creation failed" });

            return Ok(new { message = "Category created successfully" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateName([FromQuery] Guid categoryId, [FromQuery] string categoryName)
        {
            var success = await _service.UpdateAsync(categoryId, categoryName);
            if (!success)
                return NotFound(new { error = "Category not found or update failed" });

            return Ok(new { message = "Category updated successfully" });
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

