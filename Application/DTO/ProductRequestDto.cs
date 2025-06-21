using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.DTO
{
    public class ProductRequestDto
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
        public int? UserId { get; set; }
    }
}
