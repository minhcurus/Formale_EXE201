using Application.DTO;
using Application.Interface;
using Application.Service;
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
        private readonly IOutfitComboItemService _comboItemService;
        private readonly IOutfitService _outfitService;
        private readonly IUserService _userService;

        public OutfitController(IOutfitService outfitService, IOutfitComboItemService comboItemService, IUserService userService)
        {
            _outfitService = outfitService;
            _comboItemService = comboItemService;
            _userService = userService;
        }

        [Authorize]
        [HttpPost("suggest")]
        public async Task<IActionResult> SuggestCombo([FromQuery] int userId, [FromBody] string prompt)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (userId != currentUserId)
                return Forbid();

            var user = await _userService.GetUsersById(currentUserId);
            if (user.PremiumPackageId == null || user.PremiumExpiryDate < DateTime.UtcNow)
            {
                return Unauthorized("Bạn cần nâng cấp gói để sử dụng tính năng này.");  
            }

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
        public async Task<IActionResult> GetComboDetail(Guid comboId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var result = await _outfitService.GetComboDetailsAsync(comboId);
            if (result == null)
                return NotFound("Combo not found.");

            if (result.UserId != currentUserId)
                return Forbid();

            return Ok(result);
        }

        [HttpPut("update-product-in-combo")]
        public async Task<IActionResult> UpdateProductInCombo([FromBody] UpdateComboItemDto dto)
        {
            var result = await _comboItemService.UpdateProductInComboAsync(dto);
            if (!result)
                return BadRequest("Cập nhật thất bại. Kiểm tra quyền sở hữu, loại sản phẩm hoặc ID không hợp lệ.");

            return Ok("Cập nhật sản phẩm trong combo thành công.");
        }

        [Authorize]
        [HttpGet("basic-user-combos")]
        public async Task<IActionResult> GetBasicCombos([FromQuery] int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (userId != currentUserId)
                return Forbid();

            var combos = await _outfitService.GetBasicCombosByUserIdAsync(userId);
            return Ok(combos);
        }

        [Authorize]
        [HttpPut("update-combo-info")]
        public async Task<IActionResult> UpdateComboInfo([FromQuery] int userId, [FromBody] UpdateComboInfoDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (userId != currentUserId)
                return Forbid();

            var result = await _outfitService.UpdateComboInfoAsync(userId, dto);
            if (!result) return NotFound("Combo không tồn tại hoặc không thuộc quyền sở hữu.");

            return Ok(new { message = "Cập nhật thông tin combo thành công." });
        }

    }
}
