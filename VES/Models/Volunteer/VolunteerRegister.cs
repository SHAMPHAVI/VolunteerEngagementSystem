using Microsoft.AspNetCore.Mvc.Rendering;

namespace VES.Models.Volunteer
{
    public class VolunteerRegister
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ContactDetails { get; set; }
        public List<SelectListItem> AvailableInterests { get; set; }

    }
}