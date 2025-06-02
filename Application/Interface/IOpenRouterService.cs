using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IOpenRouterService
    {
        /// <summary>
        /// Đoán và trả về 1 StyleName (hoặc null nếu thất bại).
        /// </summary>
        Task<string?> ClassifyAsync(string prompt);
    }
}
