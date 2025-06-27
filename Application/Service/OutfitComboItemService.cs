using Application.DTO;
using Application.Interface;
using Domain.Entities;
using Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class OutfitComboItemService : IOutfitComboItemService
    {
        private readonly OutfitComboItemRepository _comboItemRepo;
        private readonly ProductRepository _productRepo;

        public OutfitComboItemService(OutfitComboItemRepository comboItemRepo, ProductRepository productRepo)
        {
            _comboItemRepo = comboItemRepo;
            _productRepo = productRepo;
        }
        public List<OutfitComboItem> CreateItemsFromProducts(Guid comboId, IEnumerable<Product?> products)
        {
            return products
                .Where(p => p != null)
                .Select(p => new OutfitComboItem
                {
                    Id = Guid.NewGuid(),
                    ComboId = comboId,
                    ProductId = p!.ProductId,
                    CategoryId = p.CategoryId
                })
                .ToList();
        }

        public async Task<bool> UpdateProductInComboAsync(UpdateComboItemDto dto)
        {
            var comboItem = await _comboItemRepo.GetByIdAsync(dto.ComboItemId);
            if (comboItem == null) return false;

            var newProduct = await _productRepo.GetByIdAsync(dto.ProductId);
            if (newProduct == null || newProduct.UserId != dto.UserId || newProduct.IsSystemCreated)
                return false;

            if (comboItem.CategoryId != newProduct.CategoryId)
                return false;

            comboItem.ProductId = newProduct.ProductId;
            comboItem.Product = newProduct;

            await _comboItemRepo.UpdateAsync(comboItem);
            return true;
        }
    }
}
