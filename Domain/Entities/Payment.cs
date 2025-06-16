using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Domain.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? OrderId { get; set; }
        public int? PremiumPackageId { get; set; }
        public long OrderCode { get; set; }          
        public long Amount { get; set; }             
        public string Description { get; set; }      
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerAddress { get; set; }

        public string? CancelUrl { get; set; }        
        public string ReturnUrl { get; set; }         
        public string? CancelReason { get; set; }
        public long? ExpiredAt { get; set; }        
        public string? Signature { get; set; }         
        public string? PaymentUrl { get; set; }
        public string? TransactionId { get; set; }      
        public string? CheckoutUrl { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public Status Status { get; set; } = Status.PENDING;
        public PaymentMethod Method { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public UserAccount UserAccount { get; set; }
        public Order Order { get; set; }   
        public PremiumPackage? PremiumPackage { get; set; }

    }
}
