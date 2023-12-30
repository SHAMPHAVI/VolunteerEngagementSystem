using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace VES.Models
{
    public class AlertModel
    {
        [Key]
        public string? Email {  get; set; }
        public string? BloodGroup { get; set; }

        public string? Province { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string? Team { get; set; }
    }
}
