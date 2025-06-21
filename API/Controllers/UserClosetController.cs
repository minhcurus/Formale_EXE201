using Application.Interface;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _userClosetService.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _userClosetService.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _userClosetService.DeleteByIdAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Deleted successfully" });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var data = await _userClosetService.GetByUserIdAsync(userId);
            return Ok(data);
        }
    }
}
