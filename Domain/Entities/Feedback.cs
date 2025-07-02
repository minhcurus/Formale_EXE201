using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Feedback
    {
        [Key]
        public Guid FeedbackId { get; set; }

        public string? Description { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

      
        public UserAccount User { get; set; } = null!;

        [ForeignKey("ProductId")]
        public Guid ProductId { get; set; }

        
        public Product Product { get; set; } = null!;

        public string? ImageURL { get; set; }
    }
}
