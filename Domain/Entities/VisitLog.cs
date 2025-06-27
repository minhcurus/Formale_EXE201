using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VisitLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime AccessTime { get; set; }
        public string IPAddress { get; set; } 
        public int? UserId { get; set; }
        public string? LogType {  get; set; }
        public UserAccount? User { get; set; }
    }
}
