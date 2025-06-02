using Application;
using Application.DTO;
using Application.Interface;
using Application.Service;
using Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PremiunPackageController : ControllerBase
    {
        private readonly IPremiumService _premiumService;

        public PremiunPackageController(IPremiumService premiumService)
        {
            _premiumService = premiumService;
        }

        [HttpGet]
        public async Task<ActionResult<PremiunPackageDTO>> Get()
        {
            var get = await _premiumService.GetPremiumPackages();
            return Ok(get);
        }

        [HttpPost("GetPremiumPackagesById")]
        public async Task<ActionResult> GetById(int id)
        {
            var get = await _premiumService.GetPremiumPackagesById(id);
            return Ok(get);
        }

        [HttpPost("update-package")]
        public async Task<ActionResult> UpdatePackage([FromBody]PremiunPackageDTO package)
        {
            var get = await _premiumService.UpdatePremiumPackages(package);
            return Ok(get);
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyPremium(int userId, PremiumPackageTier tier)
        {
            var result = await _premiumService.CreatePremiumOrderAndPayment(userId, tier);
            return Ok(result);
        }

        [HttpPost("update-premium")]
        public async Task<ActionResult<ResultMessage>> UpdateUserPremium([FromBody] UpdatePremiumRequest request)
        {
            var result = await _premiumService.UpdateUserPremiumAsync(request.UserId, request.PremiumPackageId);
            return Ok(result);
        }

    }
}
