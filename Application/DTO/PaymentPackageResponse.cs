using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Application.DTO
{
    public class PaymentPackageResponse
    {
        public int? UserId { get; set; }
        public int? OrderId { get; set; }
        public int? PremiumPackageId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public string? TransactionId { get; set; }
        public long OrderCode { get; set; }       
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerAddress { get; set; }
        public PaymentMethod Method { get; set; }
        public string ReturnUrl { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
