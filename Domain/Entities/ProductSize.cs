using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;

namespace Domain.Entities
{
    public class ProductSize : BaseEntity
    {
        [Key]
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public ICollection<ProductCategorySize> ProductCategorySizes { get; set; }
    }
}
