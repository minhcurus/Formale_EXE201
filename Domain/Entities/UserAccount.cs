using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserAccount
    {
        [Key]
        public int UserId {  get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string? Token { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RoleId { get; set; }
        public string? Image_User { get; set; }
        public string? Background_Image { get; set; }
        public string? Description { get; set; }
        public string? LoginProvider { get; set; } // "Local" hoặc "Google"
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string IsActive { get; set; }
        public string Status { get; set; }
        public string? otp {  get; set; }
        public DateTime? OtpExpiry { get; set; }
        public int? PremiumPackageId { get; set; }
        public PremiumPackage? PremiumPackage { get; set; }
        public DateTime? PremiumExpiryDate { get; set; }
        public Roles Role { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Payment>? Payments { get; set; }

    }
}
