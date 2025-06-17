using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;

namespace Application.Interface
{
    public interface IProductCategoryService
    {
        Task<List<(Guid CategoryId, string CategoryName)>> GetAllAsync();
        Task<string> GetNameByIdAsync(Guid id);
        Task<bool> CreateAsync(string categoryName);
        Task<bool> UpdateAsync(Guid id, string categoryName);
        Task<bool> DeleteByIdAsync(Guid id);
    }
}
