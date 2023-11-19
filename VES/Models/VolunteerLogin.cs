using System.ComponentModel.DataAnnotations;

namespace VES.Models
{
    public class VolunteerLogin
    {
        [Required(ErrorMessage = "Please enter your email.")]
        [Display(Name = "email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter your password.")]
        [Display(Name = "password")]
        public string Password { get; set; }
    }
}