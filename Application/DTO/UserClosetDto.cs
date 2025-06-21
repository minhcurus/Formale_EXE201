using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UserClosetDto
    {
        public Guid ClosetId { get; set; }
        public int UserId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ComboId { get; set; }
    }
}
