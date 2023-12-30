using System.ComponentModel.DataAnnotations;

namespace VES.Models
{
    public class Alert
    {
        [Key]
        public Guid? Id { get; set; }
        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public string? UserEmail { get; set; }
        public string? Province { get; set; }

        public string? District { get; set; }
        public string? City { get; set; }
        public string? BloodGroup { get; set; }
        public string? Team { get; set; }
        public string? Location { get; set; }
    }
}
