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
         Task<OutfitSuggestionDto?> SuggestAndSaveComboFromClosetByPromptAsync(int userId, string prompt);
        Task<OutfitComboViewDto?> GetComboDetailsAsync(Guid comboId);
    }
}
