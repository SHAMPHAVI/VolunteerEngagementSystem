using System.ComponentModel.DataAnnotations;

namespace VES.Models
{
    public class VolunteerRegister
    {


        [Required(ErrorMessage = "Please enter your full name.")]
        [Display(Name = "Full_Name")]

        public string Full_Name { get; set; }
        [Key]
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Skills (comma-separated)")]
        public string Skills { get; set; }

        [Display(Name = "Availability")]
        public string Availability { get; set; }

        [Display(Name = "Interests (comma-separated)")]
        public string Interests { get; set; }
        public string? Role { get; set; }
    }
}
