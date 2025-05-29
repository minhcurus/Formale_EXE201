using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;

namespace Domain.Entities
{
    public class ProductColor : BaseEntity
    {
        [Key]
        public Guid ColorId { get; set; }
        public string ColorName { get; set; }
    }
}
