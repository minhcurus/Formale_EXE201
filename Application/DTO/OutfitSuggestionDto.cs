using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class OutfitSuggestionDto
    { 
        public string Style { get; set; } = default!;
        public ProductResponseDto Tops { get; set; } = default!;
        public ProductResponseDto Bottoms { get; set; } = default!;
        public ProductResponseDto Footwears { get; set; } = default!;
        public ProductResponseDto Accessories { get; set; } = default!;
    }
}
