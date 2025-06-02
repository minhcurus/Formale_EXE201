using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UpdatePremiumRequest
    {
        public int UserId { get; set; }
        public int PremiumPackageId { get; set; }
    }
}
