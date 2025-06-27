using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UpdateComboItemDto
    {
        public Guid ComboItemId { get; set; }
        public Guid ProductId { get; set; }
        public int UserId { get; set; }
    }
}
