using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enum;

namespace Application.DTO
{
    public class PaymentDTO
    {
        public int UserId { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
        public string ReturnUrl { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerAddress { get; set; }
        public PaymentMethod Method { get; set; }

        public int? OrderId { get; set; }
        public string? TransactionId { get; set; }
        public long OrderCode { get; set; }
    }

}
