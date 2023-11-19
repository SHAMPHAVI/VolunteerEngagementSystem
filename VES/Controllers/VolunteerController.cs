using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VES.Data;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VES.Models;

namespace VES.Controllers
{
    public class VolunteerController : Controller
    {
        private MyDbContext _myDbContext;
        public VolunteerController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(VolunteerRegister model)
        {
            if (ModelState.IsValid)
            {
                InsertUserDataIntoDatabase(model);
                return RedirectToAction("Home");
            }

            return View(model);
        }

        public IActionResult Home()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();

            return RedirectToAction("Index");
        }
        private void InsertUserDataIntoDatabase(VolunteerRegister model)
        {
            model.Role = "Volunteer";
            try
                {
                    _myDbContext.Volunteers.Add(model);
                    _myDbContext.SaveChanges();
                    string data = model.Email;
                    HttpContext.Session.SetString("email", data);
            }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving your data.");
                }

        }
        [HttpPost]
        public IActionResult Login(VolunteerLogin model)
        {
            if (ModelState.IsValid)
            {
                VolunteerRegister volunteer = _myDbContext.Volunteers.FirstOrDefault(v => v.Email == model.Email);
                if (volunteer != null && (model.Password== volunteer.Password))
                {
                    string data = model.Email;
                    HttpContext.Session.SetString("email", data);
                    return RedirectToAction("Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Incorrect email or password.");
                }
            }

            return View();
        }

    }
}
