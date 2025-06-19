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
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
        private readonly CurrentUserService _currentUser;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserRepository repository, IMapper mapper, ICloudinaryService cloudinaryService, IOptions<JwtSetting> jwtSetting, CurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _jwtSettings = jwtSetting.Value;
            _currentUser = currentUserService;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<bool> BanUser(int id)
        {
            var check = await _repository.GetById(id);
            if (check == null)
            {
                return false;

            }

            check.Status = "Ban";
            await _repository.UpdateAsync(check);
            return true;
        }

        public async Task<bool> UnBanUser(int id)
        {
            var check = await _repository.GetById(id);
            if (check == null)
            {
                return false;

            }

            check.Status = "Active";
            await _repository.UpdateAsync(check);
            return true;
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
            var currentUserId = _currentUser.UserId;
            var user = await _repository.GetById((int)currentUserId);
            if (user == null)
            {
                return 0;
            }

            // Cập nhật thông tin text
            _mapper.Map(userDTO, user);

            // Upload file 
            if (userDTO.imageUser != null)
                user.Image_User = await _cloudinaryService.UploadImageAsync(userDTO.imageUser);

            if (userDTO.imageBackground != null)
                user.Background_Image = await _cloudinaryService.UploadImageAsync(userDTO.imageBackground);

            user.UpdateAt = DateTime.Now;

            var result = await _repository.UpdateAsync(user);

            return result;
        }

        public async Task<ResultMessage> GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Người dùng chưa đăng nhập",
                    Data = null
                };
            }

            return new ResultMessage
            {
                Success = true,
                Message = "Tìm thấy người dùng",
                Data = new
                {
                _currentUser.UserId,
                _currentUser.FullName,
                _currentUser.Email,
                _currentUser.UserName,
                _currentUser.PhoneNumber,
                _currentUser.Role,
                _currentUser.Address,
                _currentUser.DateOfBirth,
                _currentUser.ImageUser,
                _currentUser.ImageBackground,
                _currentUser.Description,
                }
            };
        }

        public async Task<UserResponse> UpdateUserPremium(UserResponse user)
        {
            var existingUser = await _repository.GetById(user.UserId);
            if (existingUser == null)
                return null;

            existingUser.PremiumPackageId = user.PremiumPackageId;
            existingUser.PremiumExpiryDate = user.PremiumExpiryDate;
            existingUser.UpdateAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existingUser);

            // Fix: Map the updated UserAccount entity to a UserResponse DTO before returning  
            return _mapper.Map<UserResponse>(existingUser);
        }

    }
}
