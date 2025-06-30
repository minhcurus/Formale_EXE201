using Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IUserClosetService
    {
        Task<List<UserClosetDto>> GetAllAsync();
        Task<UserClosetDto> GetByIdAsync(Guid id);
        Task<List<UserClosetDto>> GetByUserIdAsync(int userId);
        Task<bool> DeleteByIdAsync(Guid id);

        Task<List<UserClosetDto>> GetSingleItemsAsync(int userId);
        Task<List<UserClosetDto>> GetComboItemsAsync(int userId);
        Task<List<ClosetItemResponseDto>> SearchClosetAsync(int userId, string? keyword);
    }
}
