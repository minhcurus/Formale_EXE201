using Application.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto> GetByIdAsync(Guid id);
        Task<ProductResponseDto> CreateProductAsync(ProductRequestDto p);
        Task<ProductResponseDto> UpdateAsync(Guid id, ProductRequestDto product);
        Task<string> UpdateProductImageAsync(Guid productId, IFormFile imageFile);
        Task<bool> DeleteAsync(Guid id);
        Task<PaginatedResultDto<ProductResponseDto>> SearchAsync(ProductQueryDto dto);
        Task<OutfitSuggestionDto?> SuggestOutfitAsync(string prompt);
    }
}
