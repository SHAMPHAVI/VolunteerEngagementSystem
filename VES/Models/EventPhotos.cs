using System;
using System.ComponentModel.DataAnnotations;

namespace VES.Models
{
    public class EventPhotos
    {
        [Key]
        public Guid? Id { get; set; }
        public string UserEmail { get; set; }
        public string ImagePath { get; set; }
        public DateTime Timestamp { get; set; }
        public string Title { get; set; }

    }
}
