using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VES.Data;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VES.Models;
using static VES.Controllers.OpportunityController;

namespace VES.Controllers
{
    public class VolunteerController : Controller
    {
        private MyDbContext _myDbContext;
        public VolunteerController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }


    
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult OrgRegister()
        {
            return View();
        }
        [HttpPost]
        public IActionResult OrgRegister(VolunteerRegister model)
        {
            model.Role = "Organization";

                if (ModelState.IsValid && InsertUserDataIntoDatabase(model))
                {
                    return RedirectToAction("Home");
                }
                return View(model);
           
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(VolunteerRegister model)
        {
            model.Role = "Volunteer";

                if (ModelState.IsValid && InsertUserDataIntoDatabase(model))
                {
                    return RedirectToAction("Home");
                }

                return View(model);
            
        }
        private bool IsDuplicateKeyError(DbUpdateException ex)
        {
            var error = ex.InnerException.Message;
            if (error.Contains("Duplicate entry"))
            {
                return true;
            }
            else
            {
                return false;
            }
            
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
        private bool InsertUserDataIntoDatabase(VolunteerRegister model)
        {
            try
            {
                _myDbContext.Volunteers.Add(model);
                _myDbContext.SaveChanges();
                string data = model.Email;
                HttpContext.Session.SetString("email", data);
                return true; 
            }
            catch (DbUpdateException ex)
            {
                if (IsDuplicateKeyError(ex))
                {
                    ModelState.AddModelError("", "The email address is already in use. Please log in if you have an account or register with a different email address.");

                }
                else
                {
                    ModelState.AddModelError("", "Please verify entered details and ensure all required fields are filled.");

                }

                return false; 
            }
        }

        public IActionResult Login()
        {
            return View();
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
        [HttpPost]
        public IActionResult AddRating(ParticipantRating model)
        {
            Random random = new Random();
            model.UserEmail= HttpContext.Session.GetString("email");
            model.ID = random.Next();
            model.Date = DateTime.Now.Date;
            var existingRating = _myDbContext.ParticipantRating
            .FirstOrDefault(pr => pr.Participant == model.Participant && pr.EventName == model.EventName);
            if (existingRating != null)
            {
                existingRating.Rating = model.Rating;
                existingRating.Date = model.Date;
            }
            else
            {
                _myDbContext.ParticipantRating.Add(model);
            }
            _myDbContext.SaveChanges();
            return RedirectToAction("Participant_Rating","Opportunity");
        }
        public IActionResult Event_Rating()
        {
            DateTime today = DateTime.Now.Date;
            var user = HttpContext.Session.GetString("email");
            List<OpportunityModel> joinedEvents = new List<OpportunityModel>();
            var regEvents = _myDbContext.EventRegistrations.Where(o => o.UserEmail == user).ToList();
            foreach (var reg in regEvents)
            {
                var title = reg.EventName;
                var events = _myDbContext.Opportunities.Where(o => o.Title == title);
                joinedEvents.AddRange(events);
            }
            var pastEvents = joinedEvents.Where(o => o.Date < today).ToList();
            var eventRatings = new List<EventRatingViewModel>();

            foreach (var eventRegistration in pastEvents)
            {
                var rating = _myDbContext.EventRating
                    .FirstOrDefault(pr => pr.EventName == eventRegistration.Title);

                var ViewModel = new EventRatingViewModel
                {
                    EventName = eventRegistration.Title,
                    EventEmail = eventRegistration.UserEmail,
                    Rating = rating != null ? rating.Rating : null
                };

                eventRatings.Add(ViewModel);
            }

            return View(eventRatings);
        }
        public class EventRatingViewModel
        {
            public string? EventName { get; set; }
            public string? EventEmail { get; set; }
            public int? Rating { get; set; }
        }
        [HttpPost]
        public IActionResult UpdateRating(RateModel model)
        {
            Random random = new Random();
            model.UserEmail = HttpContext.Session.GetString("email");
            model.Id = Guid.NewGuid();
            model.Date = DateTime.Now.Date;
            var existingRating = _myDbContext.EventRating
            .FirstOrDefault(pr => pr.UserEmail == model.UserEmail && pr.EventName == model.EventName);
            if (existingRating != null)
            {
                existingRating.Rating = model.Rating;
                existingRating.Date = model.Date;
            }
            else
            {
                _myDbContext.EventRating.Add(model);
            }
            _myDbContext.SaveChanges();
            return RedirectToAction("Event_Rating", "Volunteer");
        }
    }
}
