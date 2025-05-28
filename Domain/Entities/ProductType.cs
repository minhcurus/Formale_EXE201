using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;

namespace Domain.Entities
{
    public class ProductType : BaseEntity
    {
        [Key]
        public int TypeId { get; set; }
        public string TypeName { get; set; }
    }
}
