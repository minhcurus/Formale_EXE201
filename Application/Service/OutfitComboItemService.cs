using Application.Interface;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class OutfitComboItemService : IOutfitComboItemService
    {
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
    }
}
