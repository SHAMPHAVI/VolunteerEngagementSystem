using Microsoft.AspNetCore.Mvc;
using VES.Data;
using VES.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;

namespace VES.Controllers
{
    public class AdminController : Controller
    {
        private MyDbContext _myDbContext;
        public AdminController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(AdminLogin model)
        {
            if (ModelState.IsValid)
            {
                AdminLogin admin = _myDbContext.Admin.FirstOrDefault(v => v.Email == model.Email);
                if (admin != null && (model.Password == admin.Password))
                {
                    string data = model.Email;
                    HttpContext.Session.SetString("email", data);
                    return RedirectToAction("Users","Dashboard");
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
