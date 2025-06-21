using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OutfitCombo  // BỘ TRANG PHỤC
    {
        [Key]
        public Guid ComboId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public int UserId { get; set; }
        public UserAccount User { get; set; }

        public ICollection<OutfitComboItem> Items { get; set; }
        public ICollection<UserCloset> UserClosets { get; set; }
    }
}
