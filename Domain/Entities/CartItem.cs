using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public int CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public decimal Price { get; set; }
        [JsonIgnore]
        public string? Note { get; set; }
        [JsonIgnore]
        public Cart Cart { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }

    }

}
