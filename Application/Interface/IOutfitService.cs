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
        Task<Guid> SaveSuggestedComboAsync(int userId, OutfitSuggestionDto suggestion);
        Task<OutfitComboViewDto?> GetComboDetailsAsync(Guid comboId);
    }
}
