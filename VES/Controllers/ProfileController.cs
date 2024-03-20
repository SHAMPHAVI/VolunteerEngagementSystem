using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VES.Data;
using VES.Models;

namespace VES.Controllers
{
    public class ProfileController:Controller
    {
        private MyDbContext _myDbContext;
        public ProfileController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }
        private bool IsUserAuthenticated(string userEmail)
        {
            return _myDbContext.Volunteers.Any(a => a.Email == userEmail);
        }
        private RedirectToActionResult HomePage()
        {
            return RedirectToAction("Index", "Home");
        }
        public IActionResult User(VolunteerRegister volunteer)
        {
            string userEmail = HttpContext.Session.GetString("email");
            volunteer = _myDbContext.Volunteers.FirstOrDefault(v => v.Email == userEmail);
            if (volunteer!=null && volunteer.Role == "Organization") {
                return View("Organization", volunteer);
            }
            if (volunteer != null && volunteer.Role == "Volunteer")
            {
                return View("Volunteer", volunteer);
            }
                return HomePage();

        }
        [HttpGet]
        public IActionResult Edit()
        {
            string userEmail = HttpContext.Session.GetString("email");
            VolunteerRegister volunteer = _myDbContext.Volunteers.FirstOrDefault(v => v.Email == userEmail);

            if (volunteer != null && volunteer.Role == "Organization")
            {
                return View("EditOrganization", volunteer);
            }
            if (volunteer != null && volunteer.Role == "Volunteer")
            {
                return View("EditVolunteer", volunteer);
            }
            return HomePage();
        }

        [HttpPost]
        public IActionResult EditProfile(VolunteerRegister updatedVolunteer)
        {
            ModelState.Remove("Email");
            ModelState.Remove("Role");
            ModelState.Remove("Password");
            string userEmail = HttpContext.Session.GetString("email");
            VolunteerRegister existingVolunteer = _myDbContext.Volunteers.FirstOrDefault(v => v.Email == userEmail);
            if (ModelState.IsValid)
            {
                
                if (existingVolunteer != null)
                {
                    existingVolunteer.Full_Name = updatedVolunteer.Full_Name;
                    existingVolunteer.Date = updatedVolunteer.Date;
                    existingVolunteer.Category = updatedVolunteer.Category;
                    _myDbContext.SaveChanges();
                    VolunteerRegister volunteer = _myDbContext.Volunteers.FirstOrDefault(v => v.Email == userEmail);
                    if (volunteer.Role == "Organization")
                    {
                        return View("Organization", volunteer);
                    }
                    if (volunteer.Role == "Volunteer")
                    {
                        return View("Volunteer", volunteer);
                    }
                }
            }
            return View(updatedVolunteer);
        }
        public IActionResult UpdateAlertInfo()
        {
            var email= HttpContext.Session.GetString("email");
            var existingUser = _myDbContext.AlertInfo.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                return View("EditAlertInfo",existingUser);
            }
            return View();
        }
        public IActionResult AlertInfo()
        {
            string user = HttpContext.Session.GetString("email");
            var alertInfo = _myDbContext.AlertInfo.FirstOrDefault(u => u.Email == user);
            return View();
        }
        [HttpPost]
        public IActionResult UpdateInfo(AlertModel alertModel)
        {
            alertModel.Email = HttpContext.Session.GetString("email");
            if (ModelState.IsValid)
            {
                var existingUser = _myDbContext.AlertInfo.FirstOrDefault(u => u.Email == alertModel.Email);

                if (existingUser == null)
                {
                    _myDbContext.AlertInfo.Add(alertModel);
                }
                else
                {
                    existingUser.BloodGroup = alertModel.BloodGroup;
                    existingUser.Province = alertModel.Province;
                    existingUser.District = alertModel.District;
                    existingUser.City = alertModel.City;
                    existingUser.Team = alertModel.Team;

                    _myDbContext.Entry(existingUser).State = EntityState.Modified;
                }
                _myDbContext.SaveChanges();

                return RedirectToAction("UpdateAlertInfo"); 
            }
            return View(alertModel);
        }
    }
}
