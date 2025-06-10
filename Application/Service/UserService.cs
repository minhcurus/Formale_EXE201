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
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Service
{
    public class UserService : IUserService
    {
        private readonly UserRepository _repository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;
        private readonly JwtSetting _jwtSettings;

        public UserService(UserRepository repository, IMapper mapper, ICloudinaryService cloudinaryService, IOptions<JwtSetting> jwtSetting)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _jwtSettings = jwtSetting.Value;
        }

        public async Task<bool> DeleteProfile(int id)
        {
            var check = await _repository.GetById(id);
            if (check == null)
            {
                return false;

            }

            var delete = await _repository.RemoveAsync(check);
            return delete;
        }

        public async Task<List<UserResponse>> GetAllUser()
        {
            var users = await _repository.GetAllAsync();
            return _mapper.Map<List<UserResponse>>(users);
        }

        public async Task<UserResponse> GetUsersById(int id)
        {
            var user = await _repository.GetById(id);
            var map = _mapper.Map<UserResponse>(user);
            return map;
        }

        public async Task<int> UpdateProfile(UserDTO userDTO)
        {
            var user = await _repository.GetById(userDTO.UserId);
            if (user == null)
            {
                return 0;
            }

            // Cập nhật thông tin text
            _mapper.Map(userDTO, user);

            // Upload file 
            if (userDTO.Image_User != null)
                user.Image_User = await _cloudinaryService.UploadImageAsync(userDTO.Image_User);

            if (userDTO.Background_Image != null)
                user.Background_Image = await _cloudinaryService.UploadImageAsync(userDTO.Background_Image);

            user.UpdateAt = DateTime.Now;

            var result = await _repository.UpdateAsync(user);

            return result;
        }

        public async Task<ResultMessage> GetCurrentUser(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var claims = principal.Claims;

                var result = new
                {
                    UserId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                    Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                    FullName = claims.FirstOrDefault(c => c.Type == "full_name")?.Value,
                    UserName = claims.FirstOrDefault(c => c.Type == "username")?.Value,
                    PhoneNumber = claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value,
                    Role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                    Address = claims.FirstOrDefault(c => c.Type == "address")?.Value,
                    DateOfBirth = claims.FirstOrDefault(c => c.Type == "dob")?.Value,
                    Image = claims.FirstOrDefault(c => c.Type == "image_user")?.Value,
                    BackgroundImage = claims.FirstOrDefault(c => c.Type == "background_image")?.Value,
                    Description = claims.FirstOrDefault(c => c.Type == "description")?.Value,
                };

                return new ResultMessage
                {
                    Success = true,
                    Message = "Tìm thấy người dùng",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Token không hợp lệ: " + ex.Message,
                    Data = null
                };
            }
        }

    }
}
