using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class ClosetItemResponseDto
    {
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }

        public Guid? ComboId { get; set; }
        public string? ComboName { get; set; }

        public bool IsCombo => ComboId.HasValue;
    }
}
