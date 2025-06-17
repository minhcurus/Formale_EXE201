using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IProductStyleService
    {
        Task<List<(Guid StyleId, string StyleName)>> GetAllAsync();
        Task<string> GetNameByIdAsync(Guid id);
        Task<bool> CreateAsync(string styleName);
        Task<bool> UpdateAsync(Guid id, string styleName);
        Task<bool> DeleteByIdAsync(Guid id);
    }
}
