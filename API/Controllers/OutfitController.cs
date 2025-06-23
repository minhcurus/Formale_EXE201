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
        public async Task<IActionResult> SaveCombo([FromQuery] int userId, [FromBody] OutfitSuggestionDto suggestion)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (userId != currentUserId)
                return Forbid();

            var comboId = await _outfitService.SaveSuggestedComboAsync(userId, suggestion);
            return Ok(new { ComboId = comboId, Message = "Lưu combo thành công." });
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
