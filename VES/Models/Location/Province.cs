using System.ComponentModel.DataAnnotations;

namespace VES.Models.Location
{
    public class Province
    {
        [Key]
        public int id { get; set; }
        public string name_en { get; set; }
    }
}
