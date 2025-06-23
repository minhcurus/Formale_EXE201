using Application.DTO;
using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutfitController : Controller
    {
        private readonly IOutfitService _outfitService;

        public OutfitController(IOutfitService outfitService)
        {
            _outfitService = outfitService;
        }

        [Authorize]
        [HttpPost("suggest")]
        public async Task<IActionResult> SuggestCombo([FromQuery] int userId, [FromBody] string prompt)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (userId != currentUserId)
                return Forbid();

            var suggestion = await _outfitService.SuggestComboFromClosetAsync(userId, prompt);
            if (suggestion == null)
                return NotFound("Không tìm thấy combo phù hợp.");

            return Ok(suggestion);
        }

        [Authorize]
        // API lưu combo khi user đồng ý
        [HttpPost("save")]
        public async Task<IActionResult> SaveSuggestedCombo([FromQuery] int userId, [FromQuery] Guid comboId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (userId != currentUserId) return Unauthorized("Không có quyền lưu combo này.");

            var success = await _outfitService.SaveSuggestedComboAsync(userId, comboId);
            if (!success) return NotFound("Combo không tồn tại hoặc không thuộc về người dùng.");

            return Ok(new { message = "Combo đã được lưu vào closet." });
        }

        [Authorize]
        [HttpGet("{comboId}")]
        public async Task<IActionResult> GetComboDetail([FromQuery] int userId, [FromQuery] Guid comboId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // So sánh userId từ query với userId trong token
            if (userId != currentUserId)
                return Forbid();

            var result = await _outfitService.GetComboDetailsAsync(comboId);
            if (result == null)
                return NotFound("Combo not found.");

            return Ok(result);
        }

    }
}
