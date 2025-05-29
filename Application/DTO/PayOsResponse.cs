using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Application.DTO
{
    public class PayOsResponse
    {
        [JsonPropertyName("orderCode")]
        public long OrderCode { get; set; }

        [JsonPropertyName("cancellationReason")]
        public string Desc { get; set; }

        [JsonPropertyName("data")]
        public PayOsDTO Data { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }
}
