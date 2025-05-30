using Application.DTO;
using Application.Interface;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductsController(IProductService service) => _service = service;

        // GET /api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        // GET /api/products/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            return product is null
                ? NotFound(new { message = "Product not found." })
                : Ok(product);
        }

        // POST /api/products
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductResponseDto>> Create([FromForm] ProductRequestDto dto)
        {
            try
            {
                var created = await _service.CreateProductAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, new
                {
                    message = "Product created successfully.",
                    data = created
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/products/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProductResponseDto>> Update(Guid id, [FromBody] ProductUpdateDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                return updated is null
                    ? NotFound(new { message = "Product not found." })
                    : Ok(new
                    {
                        message = "Product updated successfully.",
                        data = updated
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/products/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "Product deleted successfully." })
                : NotFound(new { message = "Product not found." });
        }

        // GET /api/products/search
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResultDto<ProductResponseDto>>> Search([FromQuery] ProductQueryDto query)
            => Ok(await _service.SearchAsync(query));
    }

}
