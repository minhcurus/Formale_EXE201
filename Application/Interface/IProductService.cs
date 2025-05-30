using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;

namespace Application.Interface
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto> GetByIdAsync(Guid id);
        Task<ProductResponseDto> CreateProductAsync(ProductRequestDto p);
        Task<ProductResponseDto> UpdateAsync(Guid id, ProductUpdateDto product);
        Task<bool> DeleteAsync(Guid id);
        Task<PaginatedResultDto<ProductResponseDto>> SearchAsync(ProductQueryDto dto);
    }
}
