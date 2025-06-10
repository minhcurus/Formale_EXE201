using System.Security.Claims;
using Application;
using Application.DTO;
using Application.Interface;
using Application.Service;
using Application.Validation;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Google;
using Application.Settings;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserAccountService _userAccountService;
        private readonly GoogleSetting _googleSetting;
        public AuthController(IUserAccountService userAccountService, IOptions<GoogleSetting> googleSetting)
        {
            _userAccountService = userAccountService;
            _googleSetting = googleSetting.Value;
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

        [HttpPost("reset-otp")]
        public async Task<IActionResult> ResetOtp([FromBody] ResetOtpDTO resetOtpDTO)
        {
            var result = await _userAccountService.ResetOtp(resetOtpDTO);
            return Ok(result);
        }

        [HttpGet("GoogleCallback")]
        public async Task<IActionResult> GoogleCallback(
            [FromQuery] string? code = null,
            [FromQuery] string? error = null,
            [FromQuery] string? state = null)
        {
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = $"Google login failed: {error}",
                    Error = error
                });
            }

            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Missing code parameter."
                });
            }

            // TODO: Gửi code về service để lấy token và xử lý tiếp
            var result = await _userAccountService.LoginWithGoogleCode(code);

            return Ok(result);
        }



        // Trả link Google login cho FE
        [HttpGet("google-login-link")]
        public IActionResult GetGoogleLoginLink()
        {
            var redirectUrl = "http://localhost:5038/api/Auth/GoogleCallback";

            var googleAuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                $"client_id={_googleSetting.ClientId}" +
                $"&response_type=code" +
                $"&scope=email%20profile" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUrl)}";

            return Ok(new { loginUrl = googleAuthUrl });
        }


        // FE gọi API này gửi Google access token/id_token cho backend
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO googleLoginDTO)
        {
            var result = await _userAccountService.GoogleLogin(googleLoginDTO.Id_Token);
            return Ok(result);
        }

    }
}
