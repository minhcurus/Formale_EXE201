using Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IOutfitService
    {
        Task<OutfitSuggestionDto?> SuggestComboFromClosetAsync(int userId, string prompt);
        Task<bool> SaveSuggestedComboAsync(int userId, Guid comboId);
        Task<OutfitComboViewDto?> GetComboDetailsAsync(Guid comboId);
    }
}
