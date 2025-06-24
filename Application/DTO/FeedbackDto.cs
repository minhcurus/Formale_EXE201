using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class FeedbackDto
    {
        public Guid FeedbackId { get; set; }
        public string? Description { get; set; }
        public int Rating { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
    }
}
