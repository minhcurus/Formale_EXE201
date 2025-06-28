using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class OutfitComboBasicDto
    {
        public Guid ComboId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
    }
}
