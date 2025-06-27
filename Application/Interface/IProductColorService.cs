using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IProductColorService
    {
        Task<List<(Guid, string)>> GetAllAsync();
        Task<string> GetNameByIdAsync(Guid id);
        Task<bool> CreateAsync(string colorName);
        Task<bool> UpdateAsync(Guid id, string colorName);
        Task<bool> DeleteByIdAsync(Guid id);
    }
}
