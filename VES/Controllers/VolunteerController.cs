using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VES.Models.Volunteer;
using System.Collections.Generic;

namespace VES.Controllers
{
    public class VolunteerController : Controller
    {
        public ActionResult Register()
        {
            var model = new VolunteerRegister
            {
                AvailableInterests = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Cleanups" },
                    new SelectListItem { Value = "2", Text = "Animals" },
                }
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Register(VolunteerRegister volunteer)
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
        {
            return View();
        }
    }
}
