using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;

namespace Domain.Entities
{
    public class ProductCategorySize : BaseEntity
    {
        public Guid CategoryId { get; set; }
        public ProductCategory Category { get; set; }

        public Guid SizeId { get; set; }
        public ProductSize Size { get; set; }
    }
}
