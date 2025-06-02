using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Application.Service
{
    public class UserService : IUserService
    {
        private readonly UserRepository _repository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public UserService(UserRepository repository, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
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

    }
}
