using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DTO
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public string Brand { get; set; }

        public string Color { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public string Style { get; set; }
        public string Category { get; set; }
        public string Material { get; set; }
        public string Type { get; set; }
    }
}
