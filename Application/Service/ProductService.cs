using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using VaccinceCenter.Repositories.Base;

namespace Application.Service
{
    public class ProductService : IProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ProductBrandRepository _productBrandRepository;
        private readonly ProductCategoryRepository _productCategoryRepository;
        private readonly ProductColorRepository _productColorRepository;
        private readonly ProductMaterialRepository _productMaterialRepository;
        private readonly ProductSizeRepository _productSizeRepository;
        private readonly ProductStyleRepository _productStyleRepository;
        private readonly ProductTypeRepository _productTypeRepository;




        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public ProductService(ProductRepository productRepository,
            ProductBrandRepository productBrandRepository,
            ProductCategoryRepository productCategoryRepository,
            ProductColorRepository productColorRepository,
            ProductMaterialRepository productMaterialRepository,
            ProductSizeRepository productSizeRepository,
            ProductStyleRepository productStyleRepository,
            ProductTypeRepository productTypeRepository,
            ICloudinaryService cloudinaryService, 
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productBrandRepository = productBrandRepository;
            _productCategoryRepository = productCategoryRepository;
            _productColorRepository = productColorRepository;
            _productMaterialRepository = productMaterialRepository;
            _productSizeRepository = productSizeRepository;
            _productStyleRepository = productStyleRepository;
            _productTypeRepository = productTypeRepository;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();

            var brands = await _productBrandRepository.GetAllAsync();
            var categories = await _productCategoryRepository.GetAllAsync();
            var colors = await _productColorRepository.GetAllAsync();
            var materials = await _productMaterialRepository.GetAllAsync();
            var styles = await _productStyleRepository.GetAllAsync();
            var types = await _productTypeRepository.GetAllAsync();

            var result = products.Select(product =>
            {
                var dto = _mapper.Map<ProductResponseDto>(product);

                dto.Brand = brands.FirstOrDefault(x => x.BrandId == product.BrandId)?.BrandName ?? "";
                dto.Category = categories.FirstOrDefault(x => x.CategoryId == product.CategoryId)?.CategoryName ?? "";
                dto.Color = colors.FirstOrDefault(x => x.ColorId == product.ColorId)?.ColorName ?? "";
                dto.Material = materials.FirstOrDefault(x => x.MaterialId == product.MaterialId)?.MaterialName ?? "";
                dto.Style = styles.FirstOrDefault(x => x.StyleId == product.StyleId)?.StyleName ?? "";
                dto.Type = types.FirstOrDefault(x => x.TypeId == product.TypeId)?.TypeName ?? "";

                return dto;
            }).ToList();

            return result;
        }

        public async Task<ProductResponseDto> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            var res = _mapper.Map<ProductResponseDto>(product);
            await MapperProduct(product, res);

            return res;
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductRequestDto dto)
        {

            var product = _mapper.Map<Product>(dto);
            string imageUrl = await _cloudinaryService.UploadImageAsync(dto.ImageFile);
            product.ImageURL = imageUrl;

            _productRepository.Create(product);

            var res = _mapper.Map<ProductResponseDto>(product);
            await MapperProduct(product, res);

            return res;
        }

        private async Task MapperProduct(Product product, ProductResponseDto res)
        {
            res.Brand = (await _productBrandRepository.GetByIdAsync(product.BrandId)).BrandName;
            res.Category = (await _productCategoryRepository.GetByIdAsync(product.CategoryId)).CategoryName;
            res.Color = (await _productColorRepository.GetByIdAsync(product.ColorId)).ColorName;
            res.Material = (await _productMaterialRepository.GetByIdAsync(product.MaterialId)).MaterialName;
            res.Style = (await _productStyleRepository.GetByIdAsync(product.StyleId)).StyleName;
            res.Type = (await _productTypeRepository.GetByIdAsync(product.TypeId)).TypeName;
        }

        public async Task<ProductResponseDto> UpdateAsync(Guid id, ProductUpdateDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;                     

            _mapper.Map(dto, product);
            await _productRepository.UpdateAsync(product);

            var res = _mapper.Map<ProductResponseDto>(product);
            await MapperProduct(product, res);          
            return res;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            product.IsDeleted = true;
            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<PaginatedResultDto<ProductResponseDto>> SearchAsync(ProductQueryDto dto)
        {
            var q = _productRepository.Query()            // no-tracking IQueryable<Product>
                                      .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(dto.Keyword))
                q = q.Where(p => EF.Functions.Like(p.Name, $"%{dto.Keyword}%"));

            if (dto.BrandId.HasValue) q = q.Where(p => p.BrandId == dto.BrandId);
            if (dto.ColorId.HasValue) q = q.Where(p => p.ColorId == dto.ColorId);
            if (dto.StyleId.HasValue) q = q.Where(p => p.StyleId == dto.StyleId);
            if (dto.CategoryId.HasValue) q = q.Where(p => p.CategoryId == dto.CategoryId);
            if (dto.MaterialId.HasValue) q = q.Where(p => p.MaterialId == dto.MaterialId);
            if (dto.TypeId.HasValue) q = q.Where(p => p.TypeId == dto.TypeId);

            var totalRecords = await q.CountAsync();
            if (totalRecords == 0)
                return new PaginatedResultDto<ProductResponseDto>(Array.Empty<ProductResponseDto>(), 0, 0, dto.Page, dto.PageSize);

            var page = dto.Page < 1 ? 1 : dto.Page;
            var pageSize = dto.PageSize < 1 ? 12 : dto.PageSize;

            var items = await q.OrderByDescending(p => p.CreatedAt)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .Select(p => new ProductResponseDto
                               {
                                   Id = p.ProductId,
                                   Name = p.Name,
                                   Price = p.Price,
                                   ImageURL = p.ImageURL,
                                   Brand = p.Brand.BrandName,
                                   Category = p.Category.CategoryName,
                                   Color = p.Color.ColorName,
                                   Material = p.Material.MaterialName,
                                   Style = p.Style.StyleName,
                                   Type = p.Type.TypeName
                               })
                               .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return new PaginatedResultDto<ProductResponseDto>(items, totalRecords, totalPages, page, pageSize);
        }


    }
}
