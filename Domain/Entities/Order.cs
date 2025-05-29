using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Domain.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public double TotalPrice { get; set; }
        public Status Status { get; set; } = Status.PENDING;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserAccount UserAccount { get; set; }
        public ICollection<Payment>? Payments { get; set; }

    }
}
