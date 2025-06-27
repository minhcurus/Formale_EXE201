using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly IVisitLogService _visitLogService;

        public LogController(IVisitLogService visitLogService) 
        { 
            _visitLogService = visitLogService;
        }

        [Authorize(Roles = "1")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllVisitDays()
        {
            var result = await _visitLogService.GetAllVisitDaysWithCounts();
            return Ok(result);
        }


        [Authorize(Roles = "1")]
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayVisitCount()
        {
            int count = await _visitLogService.GetTodayVisitCount();
            return Ok(new
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                TotalVisits = count
            });
        }

        [Authorize(Roles = "1")]
        [HttpGet("search-by-date")]
        public async Task<IActionResult> GetVisitCountByDate([FromQuery] string date)
        {
            if (!DateTime.TryParse(date, out DateTime parsedDate))
            {
                return BadRequest(new { Success = false, Message = "Ngày không hợp lệ. Định dạng yyyy-MM-dd." });
            }

            int count = await _visitLogService.GetVisitCountByDate(parsedDate);
            return Ok(new
            {
                Date = parsedDate.ToString("yyyy-MM-dd"),
                TotalVisits = count
            });
        }

        [Authorize(Roles = "1")]
        [HttpGet("get-newUser-this-month")]
        public async Task<IActionResult> GetRegistrationCountThisMonth()
        {
            var result = await _visitLogService.GetRegistrationsThisMonth();

            return Ok(new
            {
                Success = true,
                Month = result.Month,
                TotalRegistrations = result.Count
            });
        }


    }
}
