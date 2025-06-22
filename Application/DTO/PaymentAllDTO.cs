using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enum;

namespace Application.DTO
{
    public class PaymentAllDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? OrderId { get; set; }
        public int? PremiumPackageId { get; set; }
        public Status Status { get; set; }
        public PaymentMethod Method { get; set; }
        public long OrderCode { get; set; }
        public string? TransactionId { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerAddress { get; set; }
        public string? CancelUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string? CheckoutUrl { get; set; }
        public string? CancelReason { get; set; }
        public string? Signature { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime CreateAt { get; set; } 

    }
}
