using Application.DTO;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinceCenter.Repositories.Base;

namespace Application.Service
{
    public class OutfitService : IOutfitService
    {
        private readonly GenericRepository<Product> _productRepository;
        private readonly GenericRepository<ProductCategory> _productCategoryRepository;
        private readonly GenericRepository<ProductStyle> _productStyleRepository;
        private readonly GenericRepository<OutfitCombo> _outfitComboRepository;
        private readonly GenericRepository<UserCloset> _userClosetRepository;
        private readonly IOutfitComboItemService _outfitComboItemService;
        private readonly IOpenRouterService _ai;
        private readonly IMapper _mapper;

        public OutfitService(
            GenericRepository<Product> productRepository,
            GenericRepository<ProductCategory> productCategoryRepository,
            GenericRepository<ProductStyle> productStyleRepository,
            GenericRepository<OutfitCombo> outfitComboRepository,
            GenericRepository<UserCloset> userClosetRepository,
            IOutfitComboItemService outfitComboItemService,
            IOpenRouterService ai,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _productStyleRepository = productStyleRepository;
            _outfitComboRepository = outfitComboRepository;
            _userClosetRepository = userClosetRepository;
            _outfitComboItemService = outfitComboItemService;
            _ai = ai;
            _mapper = mapper;
        }

        public async Task<OutfitSuggestionDto?> SuggestComboFromClosetAsync(int userId, string prompt)
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

            var userProducts = await _userClosetRepository.Query()
                    .Where(uc => uc.UserId == userId && uc.ProductId != null)
                    .Include(uc => uc.Product)
                    .ThenInclude(p => p.Brand)
                    .Include(uc => uc.Product)
                    .ThenInclude(p => p.Color)
                    .Include(uc => uc.Product)
                    .ThenInclude(p => p.Style)
                    .Include(uc => uc.Product)
                    .ThenInclude(p => p.Material)
                    .Include(uc => uc.Product)
                    .ThenInclude(p => p.Type)
                    .Include(uc => uc.Product)
                    .ThenInclude(p => p.Category)
                    .Select(uc => uc.Product)
                    .Where(p => p.StyleId == style.StyleId && !p.IsDeleted)
                    .ToListAsync();

            var top = await FallbackFindAsync(catTops, style, userProducts);
            var bottom = await FallbackFindAsync(catBottoms, style, userProducts);
            var footwear = await FallbackFindAsync(catFootwear, style, userProducts);
            var accessory = await FallbackFindAsync(catAccessory, style, userProducts);

            var comboId = Guid.NewGuid();
            var products = new[] { top, bottom, footwear, accessory }.Where(p => p != null).ToList();
            var items = _outfitComboItemService.CreateItemsFromProducts(comboId, products);

            var combo = new OutfitCombo
            {
                ComboId = comboId,
                Name = $"AI Suggested Combo - {styleName}",
                Description = $"Generated from AI, from userId {userId}",
                UserId = userId,
                Items = items
            };

            await _outfitComboRepository.AddAsync(combo);


            return new OutfitSuggestionDto
            {
                Style = styleName,
                ComboId = comboId,
                Tops = top != null ? _mapper.Map<ProductResponseDto>(top) : null,
                Bottoms = bottom != null ? _mapper.Map<ProductResponseDto>(bottom) : null,
                Footwears = footwear != null ? _mapper.Map<ProductResponseDto>(footwear) : null,
                Accessories = accessory != null ? _mapper.Map<ProductResponseDto>(accessory) : null,
                MissingNotice = new List<string>
                {
                    top == null || !userProducts.Contains(top) ? "Missing Tops - Use Outfit System" : null,
                    bottom == null || !userProducts.Contains(bottom) ? "Missing Bottoms - Use Outfit System" : null,
                    footwear == null || !userProducts.Contains(footwear) ? "Missing Footwear - Use Outfit System" : null,
                    accessory == null || !userProducts.Contains(accessory) ? "Missing Accessories - Use Outfit System" : null
                }.Where(x => x != null).ToList()
            };
        }

        public async Task<bool> SaveSuggestedComboAsync(int userId, Guid comboId)
        {
            var combo = await _outfitComboRepository.Query()
                .FirstOrDefaultAsync(c => c.ComboId == comboId && c.UserId == userId);

            if (combo == null) return false;

            var userClosetCombo = new UserCloset
            {
                ClosetId = Guid.NewGuid(),
                UserId = userId,
                ComboId = comboId
            };

            await _userClosetRepository.AddAsync(userClosetCombo);
            return true;
        }


        private async Task<Product?> FallbackFindAsync(Guid categoryId, ProductStyle? style, List<Product>? userProducts)
        {
            // Ưu tiên sản phẩm của user
            var userProduct = userProducts.FirstOrDefault(p => p.CategoryId == categoryId);
            if (userProduct != null) return userProduct;

            // Nếu thiếu → fallback sản phẩm hệ thống
            return await _productRepository.Query()
            .Where(p => p.CategoryId == categoryId && p.StyleId == style.StyleId && p.IsSystemCreated && !p.IsDeleted)
            .Include(p => p.Brand)
            .Include(p => p.Color)
            .Include(p => p.Style)
            .Include(p => p.Material)
            .Include(p => p.Type)
            .Include(p => p.Category)
            .OrderBy(p => Guid.NewGuid())
            .FirstOrDefaultAsync();
        }

        public async Task<OutfitComboViewDto?> GetComboDetailsAsync(Guid comboId)
        {
            var combo = await _outfitComboRepository.Query()
                .Where(c => c.ComboId == comboId)
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Brand)
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Color)
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Style)
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Material)
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Type)
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync();

            if (combo == null) return null;

            return new OutfitComboViewDto
            {
                ComboId = combo.ComboId,
                Name = combo.Name,
                Description = combo.Description,
                UserId = combo.UserId,
                Items = combo.Items
                    .Select(i => _mapper.Map<ProductResponseDto>(i.Product))
                    .ToList()
            };
        }

        public async Task<List<OutfitComboBasicDto>> GetBasicCombosByUserIdAsync(int userId)
        {
            var combos = await _outfitComboRepository.Query()
                .Where(c => c.UserId == userId)
                .Select(c => new OutfitComboBasicDto
                {
                    ComboId = c.ComboId,
                    Name = c.Name,
                    Description = c.Description,
                    UserId = c.UserId
                })
                .ToListAsync();

            return combos;
        }
        public async Task<bool> UpdateComboInfoAsync(int userId, UpdateComboInfoDto dto)
        {
            var combo = await _outfitComboRepository.Query()
                .FirstOrDefaultAsync(c => c.ComboId == dto.ComboId && c.UserId == userId);

            if (combo == null) return false;

            combo.Name = dto.Name;
            combo.Description = dto.Description;

            await _outfitComboRepository.UpdateAsync(combo);
            return true;
        }


    }
}
