using System.ComponentModel.DataAnnotations;

namespace VES.Models
{
    public class VolunteerRegister
    {

        [Display(Name = "Full_Name")]

        public string Full_Name { get; set; }
        [Key]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]

        public string Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Skills")]
        public string? Skills { get; set; }

        [Display(Name = "Availability")]
        public string Availability { get; set; }

        [Display(Name = "Interests")]
        public string Interests { get; set; }
        public string? Role { get; set; }
    }
}
