using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCart() => Ok(await _cartService.GetCurrentCart());

        [Authorize]
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity)
        {
            return Ok(await _cartService.AddToCart(productId, quantity));
        }

        [Authorize]
        [HttpPost("minus-from-cart")]
        public async Task<IActionResult> MinusFromCart(Guid productId, int quantity)
        {
            return Ok(await _cartService.ReduceQuantity(productId, quantity));
        }

        [Authorize]
        [HttpGet("prepare-order")]
        public async Task<IActionResult> PrepareOrder()
        {
            return Ok(await _cartService.PrepareOrder());
        }

        [Authorize]
        [HttpDelete("cartItemId")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            return Ok(await _cartService.RemoveFromCart(cartItemId));
        }


    }
}
