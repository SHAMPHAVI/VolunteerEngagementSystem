using System.ComponentModel.DataAnnotations;

namespace VES.Models
{
    public class ParticipantRating
    {
        [Key]
        public int ID { get; set; }
        public string Participant { get; set; }
        public string UserEmail { get; set; }
        public string EventName { get; set; }
        public int? Rating { get; set; }  
        public DateTime? Date { get; set; }
    }
}
