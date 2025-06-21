using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserCloset
    {
        [Key]
        public Guid ClosetId { get; set; }

        public int UserId { get; set; }
        public UserAccount User { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? ComboId { get; set; }
        public OutfitCombo Combo { get; set; }

    }
}
