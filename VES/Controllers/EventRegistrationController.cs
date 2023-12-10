
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VES.Data;
using VES.Models;

namespace VES.Controllers
{
    [Route("EventRegistration")]
    [ApiController]
    public class EventRegistrationController : Controller
    {
        private readonly MyDbContext _context;

        public EventRegistrationController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult RegisterEvent(string eventName)
        {
            string userEmail = HttpContext.Session.GetString("email"); 
            if (!string.IsNullOrEmpty(eventName) && !string.IsNullOrEmpty(userEmail))
            {
                var eventRegistration = new EventRegistration
                {
                    EventName = eventName,
                    UserEmail = userEmail
                };

                if (ModelState.IsValid)
                {
                    InsertEventRegistrationDataIntoDatabase(eventRegistration);
                    return RedirectToAction("ViewAll");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid input or user not authenticated.");
            }

            return View("ViewAll");
        }
        public IActionResult ViewAll()
        {
            var opportunities = _context.Opportunities.ToList();
            return View(opportunities);
        }
        private void InsertEventRegistrationDataIntoDatabase(EventRegistration model)
        {
            try
            {
                _context.EventRegistrations.Add(model);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "An error occurred while saving your event registration data.");
            }
        }

    }
}
