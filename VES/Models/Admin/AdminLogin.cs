using System.ComponentModel.DataAnnotations;

namespace VES.Models.Admin
{
    public class AdminLogin
    {
        [Key]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
