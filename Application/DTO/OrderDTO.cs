using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Application.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public double TotalPrice { get; set; }
        public Status Status { get; set; } = Status.PENDING;
    }
}
