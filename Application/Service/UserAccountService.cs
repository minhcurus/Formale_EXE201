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
using Google.Apis.Auth;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
namespace Application.Service
{
    public class UserAccountService : IUserAccountService
    {
        private readonly UserAccountRepository _repository;
        private readonly JwtSetting _jwtSettings;
        private readonly EmailService _emailService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GoogleSetting _googleSetting;

        public UserAccountService(UserAccountRepository repository, IOptions<JwtSetting> jwtSetting, EmailService emailService, IHttpClientFactory httpClientFactory, IOptions<GoogleSetting> googleSetting)
        {
            _repository = repository;
            _jwtSettings = jwtSetting.Value;    
            _emailService = emailService;
            _httpClientFactory = httpClientFactory;
            _googleSetting = googleSetting.Value;
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

            var checkOtp = await _repository.GetOtp(changePasswordDTO.Otp);
            if (checkOtp == null) 
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
            user.otp = null;
            user.OtpExpiry = null;
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

            if (getEmail.Status == "Ban")
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Tài khoản này đã bị cấm",
                    Data = null
                };
            }

            if (getEmail.LoginProvider != "Local")
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Tài khoản này không hỗ trợ đăng nhập bằng mật khẩu. Hãy dùng Google.",
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
                LoginProvider = "Local",
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

        public async Task<ResultMessage> RegisterForManager(RegisterDTO registerDTO)
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
                LoginProvider = "Local",
                Address = registerDTO.Address,
                Image_User = null,
                Background_Image = null,
                Status = "InActive",
                Description = null,
                IsActive = "InActive",
                RoleId = 3,
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

        public async Task<ResultMessage> Logout(TokenDTO tokenDTO)
        {
            var user = await _repository.GetByToken(tokenDTO.token);
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
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim("full_name", user.FullName ?? ""),
        new Claim("username", user.UserName ?? ""),
        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
        new Claim("address", user.Address ?? ""),
        new Claim(ClaimTypes.Role, user.RoleId.ToString()),
    };

            //thêm nếu không null
            if (user.DateOfBirth != null)
                claims.Add(new Claim("dob", user.DateOfBirth.ToString("yyyy-MM-dd")));
            if (!string.IsNullOrEmpty(user.Image_User))
                claims.Add(new Claim("imageUser", user.Image_User));
            if (!string.IsNullOrEmpty(user.Background_Image))
                claims.Add(new Claim("imageBackground", user.Background_Image));
            if (!string.IsNullOrEmpty(user.Description))
                claims.Add(new Claim("description", user.Description));

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


        public async Task<ResultMessage> ActiveAccount(ActiveAccountDTO accountDTO)
        {
            var check = await _repository.GetEmail(accountDTO.email);
            if (check == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Email không tồn tại!",
                    Data = null
                };
            }

            var getOtp = await _repository.GetOtp(accountDTO.otp);
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

        public async Task<ResultMessage> ResetOtp(ResetOtpDTO resetOtpDTO)
        {            
            var check = await _repository.GetEmail(resetOtpDTO.email);
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
            await _emailService.SendEmailAsync(resetOtpDTO.email, "Mã kích hoạt tài khoản", $"Mã OTP của bạn là: {otp}");
            return new ResultMessage
            {
                Success = true,
                Message = "Đã gửi lại mã otp, hãy kiểm tra Email của bạn!",
                Data = null,
            };
        }

        public async Task<ResultMessage> GoogleLogin(string accessToken)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);
            }
            catch
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Token Google không hợp lệ",
                    Data = null
                };
            }

            var email = payload.Email;
            var fullName = payload.Name;
            var picture = payload.Picture;

            var user = await _repository.GetEmail(email);
            if (user != null)
            {
                if (user.Status == "Ban")
                {
                    return new ResultMessage
                    {
                        Success = false,
                        Message = "Tài khoản của bạn đã bị cấm.",
                        Data = null
                    };
                }

                var token = GenerateJwtToken(user);
                user.Token = token;
                await _repository.UpdateAsync(user);

                return new ResultMessage
                {
                    Success = true,
                    Message = "Đăng nhập Google thành công",
                    Data = token
                };
            }

            var newUser = new UserAccount
            {
                Email = email,
                FullName = fullName,
                LoginProvider = "Google",
                UserName = "",
                Address = "",
                Password = "",
                PhoneNumber = "",
                Image_User = picture,
                Background_Image = null,
                IsActive = "Active",
                Status = "Active",
                RoleId = 2,
                Token = null
            };

            await _repository.CreateAsync(newUser);

            var createdUser = await _repository.GetEmail(email);
            var jwt = GenerateJwtToken(createdUser);
            createdUser.Token = jwt;
            await _repository.UpdateAsync(createdUser);

            return new ResultMessage
            {
                Success = true,
                Message = "Tài khoản Google mới được tạo và đăng nhập thành công",
                Data = new AuthResponseDTO
                {
                    Token = jwt,
                    Email = createdUser.Email,
                    FullName = createdUser.FullName,
                    ImageUrl = createdUser.Image_User,
                }
            };
        }

        public async Task<ResultMessage> LoginWithGoogleCode(string code)
        {
            var clientId = _googleSetting.ClientId;
            var clientSecret = _googleSetting.ClientSecret;
            var redirectUri = _googleSetting.RedirectUri;

            var httpClient = _httpClientFactory.CreateClient();

            var values = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "redirect_uri", redirectUri },
            { "grant_type", "authorization_code" }
        };

            var content = new FormUrlEncodedContent(values);
            var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Failed to exchange code for token",
                    Data = responseString
                };
            }

            var tokenData = JsonSerializer.Deserialize<JsonElement>(responseString);
            var accessToken = tokenData.GetProperty("id_token").GetString();

            // TODO: Call Google API to get user info with access_token, then login or register user

            return new ResultMessage
            {
                Success = true,
                Message = "Google login successful.",
                Data = accessToken 
            };
        }
    }
}

