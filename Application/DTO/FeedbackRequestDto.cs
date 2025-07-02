using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class FeedbackRequestDto
    {
        public int Rating { get; set; }
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }

}
}
