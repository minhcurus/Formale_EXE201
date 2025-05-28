using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;

namespace Domain.Entities
{
    public class Product : BaseEntity
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public int BrandId { get; set; }
        public ProductBrand Brand { get; set; }

        public int ColorId { get; set; }
        public ProductColor Color { get; set; }

        public string ImageURL { get; set; }
        public string Description { get; set; }
        public int StyleId { get; set; } // PHONG CÁCH
        public ProductStyle Style { get; set; }

        public int CategoryId { get; set; }// LOẠI: ÁO, QUẦN, GIÀY, DÉP,...
        public ProductCategory Category { get; set; }

        public int MaterialId { get; set; }
        public ProductMaterial Material { get; set; }

        public int TypeId { get; set; } // KIỂU ÁO: ÁO T-SHIRT, ÁO BA LO,...
        public ProductType Type { get; set; }

        

    }
}
