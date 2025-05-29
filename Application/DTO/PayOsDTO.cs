using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Application.DTO
{
    public class PayOsDTO
    {
        [JsonPropertyName("checkoutUrl")]
        public string CheckoutUrl { get; set; }

        [JsonPropertyName("paymentLinkId")]
        public string PaymentLinkId { get; set; }

        [JsonPropertyName("orderCode")]
        public long OrderCode { get; set; }
        public string Signature { get; set; }
    }
}
