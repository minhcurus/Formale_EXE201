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
using Microsoft.AspNetCore.Http;
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
        private readonly UserClosetRepository _userClosetReposiotry;


        private readonly IOpenRouterService _ai;
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
            UserClosetRepository userClosetRepository,
            IOpenRouterService ai,
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
            _userClosetReposiotry = userClosetRepository;
            _ai = ai;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        // Helper: Query product kèm Include liên quan
        private IQueryable<Product> GetProductWithIncludes()
        {
            return _productRepository.Query()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Color)
                .Include(p => p.Material)
                .Include(p => p.Style)
                .Include(p => p.Type);
        }

        // Helper: Map các tên (Brand, Color, etc) cho ProductResponseDto từ các repo
        private async Task MapProductNamesAsync(Product product, ProductResponseDto dto)
        {
            dto.Brand = (await _productBrandRepository.GetByIdAsync(product.BrandId))?.BrandName ?? "";
            dto.Category = (await _productCategoryRepository.GetByIdAsync(product.CategoryId))?.CategoryName ?? "";
            dto.Color = (await _productColorRepository.GetByIdAsync(product.ColorId))?.ColorName ?? "";
            dto.Material = (await _productMaterialRepository.GetByIdAsync(product.MaterialId))?.MaterialName ?? "";
            dto.Style = (await _productStyleRepository.GetByIdAsync(product.StyleId))?.StyleName ?? "";
            dto.Type = (await _productTypeRepository.GetByIdAsync(product.TypeId))?.TypeName ?? "";
        }

        // Helper: Upload ảnh lên Cloudinary, xóa ảnh cũ nếu có
        private async Task<string> UpdateProductImageAsync(Product product, IFormFile newImage)
        {
            if (!string.IsNullOrEmpty(product.ImageURL))
            {
                var uri = new Uri(product.ImageURL);
                var fileName = uri.Segments.Last();
                var publicId = $"products/{fileName.Substring(0, fileName.LastIndexOf('.'))}";
                await _cloudinaryService.DeleteImageAsync(publicId);
            }
            return await _cloudinaryService.UploadImageAsync(newImage);
        }

        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var products = (await _productRepository.GetAllAsync())
        .Where(p => p.IsSystemCreated)
        .ToList();

            // Lấy dữ liệu lookup 1 lần cho tất cả sản phẩm
            var brands = (await _productBrandRepository.GetAllAsync()).ToDictionary(x => x.BrandId, x => x.BrandName);
            var colors = (await _productColorRepository.GetAllAsync()).ToDictionary(x => x.ColorId, x => x.ColorName);
            var materials = (await _productMaterialRepository.GetAllAsync()).ToDictionary(x => x.MaterialId, x => x.MaterialName);
            var styles = (await _productStyleRepository.GetAllAsync()).ToDictionary(x => x.StyleId, x => x.StyleName);
            var types = (await _productTypeRepository.GetAllAsync()).ToDictionary(x => x.TypeId, x => x.TypeName);
            var categories = (await _productCategoryRepository.GetAllAsync()).ToDictionary(x => x.CategoryId, x => x.CategoryName);

            return products.Select(product =>
            {
                var dto = _mapper.Map<ProductResponseDto>(product);
                dto.Brand = brands.TryGetValue(product.BrandId, out var b) ? b : "";
                dto.Color = colors.TryGetValue(product.ColorId, out var c) ? c : "";
                dto.Material = materials.TryGetValue(product.MaterialId, out var m) ? m : "";
                dto.Style = styles.TryGetValue(product.StyleId, out var s) ? s : "";
                dto.Type = types.TryGetValue(product.TypeId, out var t) ? t : "";
                dto.Category = categories.TryGetValue(product.CategoryId, out var cat) ? cat : "";
                return dto;
            }).ToList();
        }

        // Lấy sản phẩm theo ID
        public async Task<ProductResponseDto> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            var dto = _mapper.Map<ProductResponseDto>(product);
            await MapProductNamesAsync(product, dto);

            return dto;
        }

        // Tạo sản phẩm mới
        public async Task<ProductResponseDto> CreateProductAsync(ProductRequestDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.ImageURL = dto.ImageFile != null ? await _cloudinaryService.UploadImageAsync(dto.ImageFile): "";
            product.IsSystemCreated = true;

            await _productRepository.AddAsync(product);

            //check UserID
            if (dto.UserId.HasValue)
            {
                product.UserId = dto.UserId.Value;
                product.IsSystemCreated = false;

                var userCloset = new UserCloset
                {
                    ClosetId = Guid.NewGuid(),
                    UserId = dto.UserId.Value,
                    ProductId = product.ProductId,
                    ComboId = null
                };
                await _userClosetReposiotry.AddAsync(userCloset);
            }

            var productFull = await GetProductWithIncludes()
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            return _mapper.Map<ProductResponseDto>(productFull);
        }

        // Cập nhật sản phẩm
        public async Task<ProductResponseDto> UpdateAsync(Guid id, ProductRequestDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            _mapper.Map(dto, product);

            if (dto.ImageFile != null)
                product.ImageURL = await UpdateProductImageAsync(product, dto.ImageFile);

            await _productRepository.UpdateAsync(product);

            var productFull = await GetProductWithIncludes()
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            return _mapper.Map<ProductResponseDto>(productFull);
        }

        public async Task<string> UpdateProductImageAsync(Guid productId, IFormFile imageFile)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) throw new Exception("Product not found");

            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Invalid image file.");

            // Xoá ảnh cũ nếu cần
            if (!string.IsNullOrEmpty(product.ImageURL))
                await _cloudinaryService.DeleteImageAsync(product.ImageURL); // tuỳ bạn có xài không

            // Upload ảnh mới
            var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile);

            // Cập nhật
            product.ImageURL = imageUrl;
            await _productRepository.UpdateAsync(product);

            return imageUrl;
        }

        // Xoá mềm (soft delete)
        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            product.IsDeleted = true;
            await _productRepository.UpdateAsync(product);
            return true;
        }

        // Tìm kiếm phân trang
        public async Task<PaginatedResultDto<ProductResponseDto>> SearchAsync(ProductQueryDto dto)
        {
            var q = _productRepository.Query()
                                      .Where(p => !p.IsDeleted && p.IsSystemCreated);

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
                                   ProductId = p.ProductId,
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

        // Gợi ý combo đồ dựa trên prompt người dùng (AI)
        public async Task<OutfitSuggestionDto?> SuggestOutfitAsync(string prompt)
        {
            var styleName = await _ai.ClassifyAsync(prompt);
            if (string.IsNullOrEmpty(styleName)) return null;

            var style = await _productStyleRepository.Query()
                .FirstOrDefaultAsync(s => s.StyleName == styleName);
            if (style == null) return null;

            var categories = await _productCategoryRepository.GetAllAsync();
            var catTops = categories.First(c => c.CategoryName == "Tops").CategoryId;
            var catBottoms = categories.First(c => c.CategoryName == "Bottoms").CategoryId;
            var catFootwear = categories.First(c => c.CategoryName == "Footwear").CategoryId;
            var catAccessory = categories.First(c => c.CategoryName == "Accessories").CategoryId;

            async Task<Product?> PickAsync(Guid catId) =>
                await _productRepository.Query()
                    .Where(p => p.CategoryId == catId && p.StyleId == style.StyleId && !p.IsDeleted)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Color)
                    .Include(p => p.Material)
                    .Include(p => p.Style)
                    .Include(p => p.Type)
                    .OrderBy(p => Guid.NewGuid()) // random
                    .FirstOrDefaultAsync();

            var top = await PickAsync(catTops);
            var bottom = await PickAsync(catBottoms);
            var footwear = await PickAsync(catFootwear);
            var accessory = await PickAsync(catAccessory);

            //if (top == null || bottom == null || footwear == null)

            return new OutfitSuggestionDto
            {
                Style = style.StyleName,
                Tops = _mapper.Map<ProductResponseDto>(top),
                Bottoms = _mapper.Map<ProductResponseDto>(bottom),
                Footwears = _mapper.Map<ProductResponseDto>(footwear),
                Accessories = _mapper.Map<ProductResponseDto>(accessory) 
            };
        }
    } 
}
