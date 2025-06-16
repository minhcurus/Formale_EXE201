using Application;
using Application.DTO;
using Application.Interface;
using Application.Service;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "1")]
        [HttpPost("update-package")]
        public async Task<ActionResult> UpdatePackage([FromBody]PremiunPackageDTO package)
        {
            var get = await _premiumService.UpdatePremiumPackages(package);
            return Ok(get);
        }

        [Authorize]
        [HttpPost("buy")]
        public async Task<IActionResult> BuyPremium(PremiumPackageTier tier)
        {
            var result = await _premiumService.CreatePremiumOrderAndPayment( tier);
            return Ok(result);
        }

        [Authorize(Roles = "1")]
        [HttpPost("update-premium")]
        public async Task<ActionResult<ResultMessage>> UpdateUserPremium([FromBody] UpdatePremiumRequest request)
        {
            var result = await _premiumService.UpdateUserPremiumAsync(request.UserId, request.PremiumPackageId);
            return Ok(result);
        }

    }
}
