using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.DTO
{
    public class UserDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        [JsonPropertyName("imageUser")]
        public IFormFile? Image_User { get; set; }
        [JsonPropertyName("imageBackground")]
        public IFormFile? Background_Image { get; set; }
        public string? Description { get; set; }
    }
}
