using Application.DTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IOutfitComboItemService
    {
        List<OutfitComboItem> CreateItemsFromProducts(Guid comboId, IEnumerable<Product?> products);
        Task<bool> UpdateProductInComboAsync(UpdateComboItemDto dto);
    }
}
