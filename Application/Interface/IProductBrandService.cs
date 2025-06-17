using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IProductBrandService
    {
        Task<List<(Guid BrandId, string BrandName)>> GetAllAsync();
        Task<string> GetNameByIdAsync(Guid id);
        Task<bool> CreateAsync(string brandName);
        Task<bool> UpdateAsync(Guid id, string brandName);
        Task<bool> DeleteByIdAsync(Guid id);
    }
}
