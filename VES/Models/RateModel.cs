
using System.ComponentModel.DataAnnotations;

namespace VES.Models
{
    public class RateModel
    {

        [Key]
        public Guid? Id { get; set; }

        public string? UserEmail { get; set; }
        public int? Rating { get; set; }
        public string? EventName { get; set; }
        public string? EventEmail { get; set; }
        public DateTime? Date { get; set;}
    }
}
