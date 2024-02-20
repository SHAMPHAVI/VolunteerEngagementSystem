using System.ComponentModel.DataAnnotations;

namespace VES.Models
{
    public class CommentModel
    {
        [Key]
        public Guid? Id { get; set; }
        public string UserEmail { get; set; }
        public string CommentText { get; set; }
        public DateTime Timestamp { get; set; }
        public string Title { get; set; }
        public string type { get; set; }
        public string replyto { get; set; }
    }
}