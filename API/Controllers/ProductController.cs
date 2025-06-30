using Application.DTO;
using Application.Interface;
using Application.Service;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
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
        {
            try
            {
                var products = await _service.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving products.", error = ex.Message });
            }
        }

        // GET /api/products/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
        {
            try
            {
                var product = await _service.GetByIdAsync(id);
                return product is null
                    ? NotFound(new { message = "Product not found." })
                    : Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving product.", error = ex.Message });
            }
        }

        // POST /api/products
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductResponseDto>> Create([FromForm] ProductRequestDto dto)
        {
            try
            {
                var created = await _service.CreateProductAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, new
                {
                    message = "Product created successfully.",
                    data = created
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Create failed.",
                    error = ex.GetBaseException().Message
                });
            }
        }


        // PUT /api/products/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProductResponseDto>> Update(Guid id, [FromForm] ProductRequestDto dto)
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
                return BadRequest(new { message = "Update failed.", error = ex.Message });
            }
        }

        [HttpPut("{id}/image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateImage(Guid id, [FromForm] ProductImageUploadDto dto)
        {
            if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                return BadRequest("No image file provided.");

            var imageUrl = await _service.UpdateProductImageAsync(id, dto.ImageFile);
            return Ok(new { imageUrl });
        }

        [Authorize(Roles = "1")]
        // DELETE /api/products/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                return result
                    ? Ok(new { message = "Product deleted successfully." })
                    : NotFound(new { message = "Product not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Delete failed.", error = ex.Message });
            }
        }

        // GET /api/products/search
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResultDto<ProductResponseDto>>> Search([FromQuery] ProductQueryDto query)
        {
            try
            {
                var result = await _service.SearchAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Search failed.", error = ex.Message });
            }
        }

        // POST /api/products/suggest
        [HttpPost("suggest")]
        public async Task<IActionResult> SuggestOutfit([FromBody] string prompt)
        {
            try
            {
                var combo = await _service.SuggestOutfitAsync(prompt);
                return Ok(new
                {
                    message = "Outfit suggested successfully.",
                    data = combo
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Suggest outfit failed.", error = ex.Message });
            }
        }


    }

}
