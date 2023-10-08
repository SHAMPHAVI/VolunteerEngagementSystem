using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using VES.Data;
using VES.Models.Volunteer;
using MySqlConnector;

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

        [HttpPost]
        public IActionResult Register(VolunteerRegister model)
        {
            if (ModelState.IsValid)
            {
                InsertDataIntoDatabase(model);
                return RedirectToAction("ThankYou");
            }

            return View(model);
        }

        public IActionResult ThankYou()
        {
            return View();
        }

        private void InsertDataIntoDatabase(VolunteerRegister model)
        {
            //var connectionString = _configuration.GetConnectionString("MySQLConnection");

            //var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
            //optionsBuilder.UseSqlServer(connectionString);

            //using (var dbContext = new MyDbContext(optionsBuilder.Options))
            //{
                try
                {
                    _myDbContext.Volunteers.Add(model);
                    _myDbContext.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving your data.");
                }
            //}
        }
    }
}
