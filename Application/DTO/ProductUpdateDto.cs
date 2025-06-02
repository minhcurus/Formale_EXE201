using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DTO
{
    public class ProductUpdateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Guid BrandId { get; set; }

        public Guid ColorId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Description { get; set; }
        public Guid StyleId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid MaterialId { get; set; }
        public Guid TypeId { get; set; }
    }
}
