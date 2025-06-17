using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IProductTypeService
    {
        Task<List<(Guid TypeId, string TypeName)>> GetAllAsync();
        Task<string> GetNameByIdAsync(Guid id);
        Task<bool> CreateAsync(string typeName);
        Task<bool> UpdateAsync(Guid id, string typeName);
        Task<bool> DeleteByIdAsync(Guid id);
    }
}
