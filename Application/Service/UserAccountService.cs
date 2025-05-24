using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface;
using Application.Settings;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using static System.Net.WebRequestMethods;
namespace Application.Service
{
    public class UserAccountService : IUserAccountService
    {
        private readonly UserAccountRepository _repository;
        private readonly JwtSetting _jwtSettings;
        private readonly EmailService _emailService;

        public UserAccountService(UserAccountRepository repository, IOptions<JwtSetting> jwtSetting, EmailService emailService)
        {
            _repository = repository;
            _jwtSettings = jwtSetting.Value;    
            _emailService = emailService;
        }

        public async Task<ResultMessage> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var checkEmail = await _repository.GetEmail(changePasswordDTO.Email);
            if (checkEmail == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Email không đúng",
                    Data = null
                };
            }

            var checkToken = await _repository.GetByToken(changePasswordDTO.Otp);
            if (checkToken == null) 
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "OTP không hợp lệ",
                    Data = null
                };
            }

            var user = await _repository.GetLogin(changePasswordDTO.Email, changePasswordDTO.OldPassword);
            if (user == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Mật khẩu cũ không đúng",
                    Data = null
                };
            }

            user.Password = changePasswordDTO.NewPassword;
            await _repository.UpdateAsync(user);

            return new ResultMessage
            {
                Success = true,
                Message = "Đổi mật khẩu thành công",
                Data = null
            };
        }       

        public async Task<ResultMessage> Login(LoginDTO loginDTO)
        {
            var getEmail = await _repository.GetEmail(loginDTO.Email);
            if(getEmail == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Tài khoản không tồn tại",
                    Data = null
                };
            }

            if (getEmail.IsActive != "Active")
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Tài khoản chưa được kích hoạt, hãy kích hoạt tài khoản của bạn",
                    Data = null
                };
            }

            var getLogin = await _repository.GetLogin(loginDTO.Email,loginDTO.Password);
            if (getLogin == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Tài khoản hoặc mật khẩu không đúng",
                    Data = null
                };
            }

            var token = GenerateJwtToken(getLogin);
            getLogin.Token = token;
            await _repository.UpdateAsync(getLogin);

            return new ResultMessage
            {
                Success = true,
                Message = "Login successfully",
                Data = token
            };
        }

        public async Task<ResultMessage> Register(RegisterDTO registerDTO)
        {
            var checkEmail = await _repository.GetEmail(registerDTO.Email);
            if (checkEmail != null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Email đã tồn tại!",
                    Data = null
                };
            }

            var otp = OTPGenerator.GenerateOTP();
            var newUser = new UserAccount
            {
                Email = registerDTO.Email,
                Password = registerDTO.Password, 
                FullName = registerDTO.FullName,
                UserName = registerDTO.UserName,
                PhoneNumber = registerDTO.PhoneNumber,
                Address = registerDTO.Address,
                Image_User = null,
                Background_Image = null,
                Status = "InActive",
                Description = null,
                IsActive = "InActive",
                RoleId = 2,
                otp = otp,  
                OtpExpiry = DateTime.UtcNow.AddMinutes(10),
            };

            await _repository.CreateAsync(newUser);
            await _emailService.SendEmailAsync(registerDTO.Email, "Mã kích hoạt tài khoản", $"Mã OTP của bạn là: {otp}");
            return new ResultMessage
            {
                Success = true,
                Message = "Đăng ký thành công! \n\r Hãy kiểm tra email của bạn để nhận mã OTP kích hoạt!",
                Data = "0"
            };

        }

        public async Task<ResultMessage> Logout(string token)
        {
            var user = await _repository.GetByToken(token);
            if (user == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Token không hợp lệ",
                    Data = null
                };
            }

            user.Token = null;
            await _repository.UpdateAsync(user);

            return new ResultMessage
            {
                Success = true,
                Message = "Đăng xuất thành công",
                Data = null
            };
        }


        private string GenerateJwtToken(UserAccount user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ResultMessage> ActiveAccount(string email,string otp)
        {
            var check = await _repository.GetEmail(email);
            if (check == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Email không tồn tại!",
                    Data = null
                };
            }

            var getOtp = await _repository.GetOtp(otp);
            if (getOtp == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Mã Otp của bạn không hợp lệ",
                    Data = null
                };
            }

            if (getOtp.OtpExpiry < DateTime.UtcNow)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "OTP đã hết hạn",
                    Data = null
                };
            }

            getOtp.IsActive = "Active";
            getOtp.Status = "Active";
            getOtp.otp = null;          
            getOtp.OtpExpiry = null;    
            await _repository.UpdateAsync(getOtp);

            return new ResultMessage
            {
                Success = true,
                Message = "Kích hoạt tài khoản thành công!",
                Data = getOtp.Email,
            };

        }

        public async Task<ResultMessage> ResetOtp(string email)
        {            
            var check = await _repository.GetEmail(email);
            if (check == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Email không tồn tại!",
                    Data = null
                };
            }
            var otp = OTPGenerator.GenerateOTP();


            check.otp = otp;
            check.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
            await _repository.UpdateAsync(check);
            await _emailService.SendEmailAsync(email, "Mã kích hoạt tài khoản", $"Mã OTP của bạn là: {otp}");
            return new ResultMessage
            {
                Success = true,
                Message = "Đã gửi lại mã otp, hãy kiểm tra Email của bạn!",
                Data = null,
            };
        }
    }
}

