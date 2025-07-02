using Application.DTO;
using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create(Guid productId, [FromForm] FeedbackRequestDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            try
            {
                await _feedbackService.CreateFeedbackAsync(userId, productId, request);
                return Ok(new { message = "Feedback created." });
            }
            catch (Exception ex) when (ex.Message == "Feedback already exists.")
            {
                return Conflict(new { message = "Bạn đã đánh giá sản phẩm này rồi. Vui lòng chỉnh sửa nếu cần." });
            }
            catch
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi gửi đánh giá." });
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update(Guid productId, [FromForm] FeedbackRequestDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            await _feedbackService.UpdateFeedbackAsync(userId, productId, request);
            return Ok(new { message = "Feedback updated." });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats(Guid productId)
        {
            var stats = await _feedbackService.GetProductFeedbackStatsAsync(productId);
            return Ok(new { total = stats.TotalFeedbacks, average = stats.AverageRating });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeedbacks(Guid productId)
        {
            var list = await _feedbackService.GetProductFeedbacksAsync(productId);
            return Ok(list);
        }

        [Authorize]
        [HttpGet("my-feedbacks")]
        public async Task<IActionResult> GetMyFeedbacks()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _feedbackService.GetUserFeedbacksAsync(userId);
            return Ok(result);
        }
    }

}
