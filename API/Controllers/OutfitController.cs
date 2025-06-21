using Application.Interface;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("suggest-save-from-closet")]
        public async Task<IActionResult> SuggestAndSaveFromCloset([FromQuery] int userId, [FromBody] string prompt)
        {
            var result = await _outfitService.SuggestAndSaveComboFromClosetByPromptAsync(userId, prompt);
            if (result == null) return NotFound("No suitable combo found in closet with this style.");
            return Ok(result);
        }

        [HttpGet("{comboId}")]
        public async Task<IActionResult> GetComboDetail(Guid comboId)
        {
            var result = await _outfitService.GetComboDetailsAsync(comboId);
            if (result == null) return NotFound("Combo not found.");
            return Ok(result);
        }

    }
}
