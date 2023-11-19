using System.ComponentModel.DataAnnotations;

namespace VES.Models
{
    public class EventRegistration
    {

        [Key]
        public Guid? Id { get; set; }

        public string? EventEmail { get; set; }
        public string? UserEmail { get; set; }
        [Display(Name = "EventName")]
        public string? EventName { get; set; }
    }
}
