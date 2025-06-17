using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IProductMaterialService
    {
        Task<List<(Guid MaterialId, string MaterialName)>> GetAllAsync();
        Task<string> GetNameByIdAsync(Guid id);
        Task<bool> CreateAsync(string materialName);
        Task<bool> UpdateAsync(Guid id, string materialName);
        Task<bool> DeleteByIdAsync(Guid id);
    }
}
