using Application;
using Application.DTO;
using Application.Interface;
using Application.Service;
using Application.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserAccountService _userAccountService;

        public AccountController(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var validator = new RegisterRequestValidator();
            var validatorResult = validator.Validate(registerDTO);

            if (validatorResult.IsValid == false)
            {
                return BadRequest(new ResultMessage
                {
                    Success = false,
                    Message = "Missing value!",
                    Data = validatorResult.ToString()
                });
            }

            var result = await _userAccountService.Register(registerDTO);
            return Ok(result);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var validator = new LoginRequestValidator();
            var validatorResult = validator.Validate(loginDTO);
            if (validatorResult.IsValid == false)
            {
                return BadRequest(new ResultMessage
                {
                    Success = false,
                    Message = "Missing value!",
                    Data = validatorResult.ToString()
                });
            }

            var result = await _userAccountService.Login(loginDTO);
            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (string.IsNullOrEmpty(changePasswordDTO.Email) || string.IsNullOrEmpty(changePasswordDTO.OldPassword) || string.IsNullOrEmpty(changePasswordDTO.NewPassword))
            {
                return BadRequest(new ResultMessage
                {
                    Success = false,
                    Message = "Thiếu thông tin",
                    Data = null
                });
            }

            var result = await _userAccountService.ChangePassword(changePasswordDTO);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(TokenDTO token)
        {
            var result = await _userAccountService.Logout(token);
            return Ok(result);
        }

        [HttpPost("activate-otp")]
        public async Task<IActionResult> ActivateAccount([FromBody] ActiveAccountDTO activeAccountDTO)
        {
            if (string.IsNullOrEmpty(activeAccountDTO.otp))
            {
                return BadRequest(new ResultMessage
                {
                    Success = false,
                    Message = "Thiếu mã OTP",
                    Data = null
                });
            }

            var result = await _userAccountService.ActiveAccount(activeAccountDTO);
            return Ok(result);
        }

        [HttpPost("Reset-otp")]
        public async Task<IActionResult> ResetOtp([FromBody] ResetOtpDTO resetOtpDTO)
        {
            var result = await _userAccountService.ResetOtp(resetOtpDTO);
            return Ok(result);
        }
    }
}
