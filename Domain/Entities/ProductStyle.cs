using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;

namespace Domain.Entities
{
    public class ProductStyle : BaseEntity
    {
        [Key]
        public int StyleId { get; set; }
        public string StyleName { get; set; }
    }
}
