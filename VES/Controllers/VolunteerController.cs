using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using VES.Data;
using VES.Models.Volunteer;

namespace VES.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly IConfiguration _configuration;

        public VolunteerController(IConfiguration configuration)
        {
            _configuration = configuration;
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
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var dbContext = new MyDbContext(optionsBuilder.Options))
            {
                try
                {
                    dbContext.Users.Add(model);
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving your data.");
                }
            }
        }
    }
}
