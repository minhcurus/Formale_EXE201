using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Enum;

namespace Application.DTO
{
    public class PaymentRequestDTO
    {
        [JsonIgnore]
        public int? UserId { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
        public string ReturnUrl { get; set; }
        [JsonIgnore]
        public int? PremiumPackageId { get; set; }
        [JsonIgnore]
        public string? BuyerName { get; set; }
        [JsonIgnore]
        public string? BuyerEmail { get; set; }
        [JsonIgnore]
        public string? BuyerPhone { get; set; }
        [JsonIgnore]
        public string? BuyerAddress { get; set; }
        [JsonIgnore]
        public PaymentMethod Method { get; set; }
        [JsonIgnore]
        public int? OrderId { get; set; }
        [JsonIgnore]
        public string? TransactionId { get; set; }
        [JsonIgnore]
        public long OrderCode { get; set; }
    }
}
