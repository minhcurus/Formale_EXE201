using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;

namespace Application.Interface
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetAllUser();
        Task<UserResponse> GetUsersById(int id);
        Task<ResultMessage> UpdateProfile(UserDTO userAccount);
        Task<bool> DeleteProfile(int id);
        Task<ResultMessage> GetCurrentUser();
        Task<UserResponse> UpdateUserPremium(UserResponse user);
        Task<bool> BanUser(int id);
        Task<bool> UnBanUser(int id);
    }
}
