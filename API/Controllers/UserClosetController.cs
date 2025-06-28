using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserClosetController : Controller
    {
        private readonly IUserClosetService _userClosetService;

        public UserClosetController(IUserClosetService userClosetService)
        {
            _userClosetService = userClosetService;
        }


        [Authorize(Roles = "1")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _userClosetService.GetAllAsync();
            return Ok(data);
        }

        [Authorize(Roles = "2")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _userClosetService.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Kiểm tra closet có tồn tại và thuộc user
            var closet = await _userClosetService.GetByIdAsync(id);
            if (closet == null)
                return NotFound();

            if (closet.UserId != currentUserId)
                return Forbid();

            var success = await _userClosetService.DeleteByIdAsync(id);
            if (!success)
                return StatusCode(500, "Xóa thất bại.");

            return Ok(new { message = "Deleted successfully" });
        }


        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            if (userId != currentUserId)
                return Forbid();

            var data = await _userClosetService.GetByUserIdAsync(userId);
            return Ok(data);
        }


        [Authorize]
        [HttpGet("single-items")]
        public async Task<IActionResult> GetSingleItems()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _userClosetService.GetSingleItemsAsync(currentUserId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("combo-items")]
        public async Task<IActionResult> GetComboItems()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _userClosetService.GetComboItemsAsync(currentUserId);
            return Ok(result);
        }

    }
}
