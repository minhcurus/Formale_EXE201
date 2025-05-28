using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;

namespace Domain.Entities
{
    public class ProductBrand : BaseEntity
    {
        [Key]
        public int BrandId { get; set; }
        public string BrandName { get; set; }
    }
}
