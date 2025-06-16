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
    public class UserResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Image_User { get; set; }
        public string? Background_Image { get; set; }
        public string? Description { get; set; }
        public int? PremiumPackageId { get; set; }
        public string? LoginProvider { get; set; }
        public string IsActive { get; set; }
        public string Status { get; set; }
        public DateTime PremiumExpiryDate { get; internal set; }
    }
}
