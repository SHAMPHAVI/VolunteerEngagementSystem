using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VES.Data;
using VES.Models;
using Microsoft.AspNetCore.Authentication;
using VES.Models.Location;
using System.Linq;

namespace VES.Controllers
{
    public class OpportunityController : Controller
    {
        private MyDbContext _myDbContext;
        public OpportunityController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }
        private bool IsUserAuthenticated()
        {
            string userEmail = HttpContext.Session.GetString("email");
            return _myDbContext.Volunteers.Any(a => a.Email == userEmail);
        }
        private RedirectToActionResult HomePage()
        {
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Add()
        {
            return View();
        }
        public IActionResult Participant_Rating()
        {
            var user = HttpContext.Session.GetString("email");
            var events = _myDbContext.EventRegistrations
                .Where(e => e.EventEmail == user)
                .ToList();

            var participantRatings = new List<ParticipantRatingViewModel>();

            foreach (var eventRegistration in events)
            {
                var rating = _myDbContext.ParticipantRating
                    .FirstOrDefault(pr => pr.EventName == eventRegistration.EventName && pr.Participant == eventRegistration.UserEmail);

                var participantRatingViewModel = new ParticipantRatingViewModel
                {
                    EventName = eventRegistration.EventName,
                    Participant = eventRegistration.UserEmail,
                    Rating = rating != null ? rating.Rating : null
                };

                participantRatings.Add(participantRatingViewModel);
            }

            return View(participantRatings);
        }

        public class ParticipantRatingViewModel
        {
            public string? Participant { get; set; }
            public string? EventName { get; set; }
            public int? Rating { get; set; }
        }

        public IActionResult Details(string title)
        {
            string userEmail = HttpContext.Session.GetString("email");
            if (IsUserAuthenticated(userEmail))
            {
                var opportunityDetails = _myDbContext.Opportunities.FirstOrDefault(o => o.Title == title);
                if (opportunityDetails == null)
                {
                    return View("OpportunityNotFound");
                }

                return View(opportunityDetails);
            }
            else
            {
                return HomePage();
            }
        }
        public IActionResult JoinEvents(string title)
        {
            string userEmail = HttpContext.Session.GetString("email");
            if (IsUserAuthenticated(userEmail))
            {
                var opportunityDetails = _myDbContext.Opportunities.FirstOrDefault(o => o.Title == title);
            if (opportunityDetails == null)
            {
                return View("OpportunityNotFound");
            }

            return View(opportunityDetails);
            }
            else
            {
                return HomePage();
            }

        }
        public IActionResult PastEvent(string title)
        {
            string userEmail = HttpContext.Session.GetString("email");
            if (IsUserAuthenticated(userEmail))
            {
                var opportunityDetails = _myDbContext.Opportunities.FirstOrDefault(o => o.Title == title);
            if (opportunityDetails == null)
            {
                return View("OpportunityNotFound");
            }

            return View(opportunityDetails);
            }
            else
            {
                return HomePage();
            }
        }
        public IActionResult JoinedEvent(string title)
        {
            string userEmail = HttpContext.Session.GetString("email");
            if (IsUserAuthenticated(userEmail))
            {
                var opportunityDetails = _myDbContext.Opportunities.FirstOrDefault(o => o.Title == title);
            if (opportunityDetails == null)
            {
                return View("OpportunityNotFound");
            }

            return View(opportunityDetails);
            }
            else
            {
                return HomePage();
            }
        }


        [HttpPost]
        public IActionResult Add(OpportunityModel model)
        {
            string userEmail = HttpContext.Session.GetString("email");
            model.UserEmail = userEmail;

            if (ModelState.IsValid)
            {
                InsertOpportunityDataIntoDatabase(model);
                return RedirectToAction("Events");
            }

            return View(model);
        }
        private void InsertOpportunityDataIntoDatabase(OpportunityModel model)
        {
            try
            {
                _myDbContext.Opportunities.Add(model);
                _myDbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "An error occurred while saving your data.");
            }

        }
        private bool IsUserAuthenticated(string userEmail)
        {
            return _myDbContext.Volunteers.Any(a => a.Email == userEmail);
        }
        public IActionResult ViewAll()
        {
            var opportunities = _myDbContext.Opportunities.ToList();
            return View(opportunities);
        }
        public IActionResult AddAlert()
        {
            string userEmail = HttpContext.Session.GetString("email");
            if (IsUserAuthenticated(userEmail))
            {
                return View();
            }
            return HomePage();
        }
        [HttpPost]
        public IActionResult AddAlert(Alert model)
        {
            string userEmail = HttpContext.Session.GetString("email");
            model.UserEmail = userEmail;
            model.Id = Guid.NewGuid();
            if (ModelState.IsValid)
            {
                InsertAlertDataIntoDatabase(model);
                return RedirectToAction("ViewAlerts");
            }

            return View(model);
        }
        public IActionResult Events()
        {
            string userEmail = HttpContext.Session.GetString("email");
            if (IsUserAuthenticated(userEmail))
            {
                DateTime today = DateTime.Now.Date;
                var myEvents = _myDbContext.Opportunities.Where(o => o.UserEmail == userEmail).ToList();
                VolunteerRegister volunteer = _myDbContext.Volunteers.FirstOrDefault(v => v.Email == userEmail);
                List<OpportunityModel> joinedEvents = new List<OpportunityModel>();
                var regEvents = _myDbContext.EventRegistrations.Where(o => o.UserEmail == userEmail).ToList();
                foreach (var reg in regEvents)
                {
                    var title = reg.EventName;
                    var events = _myDbContext.Opportunities.Where(o => o.Title == title);
                    joinedEvents.AddRange(events);
                }
                var pastEvents = joinedEvents.Where(o => o.Date < today).ToList();
                var futureEvents = joinedEvents.Where(o => o.Date >= today).ToList();
                var othersEvents = _myDbContext.Opportunities.Where(o => o.UserEmail != userEmail).ToList();
                var notJoinedEvents = othersEvents.Where(o => !joinedEvents.Any(j => j.Title == o.Title)).ToList();
                var viewModel = new MyEventsViewModel
                {
                    MyEvents = myEvents,
                    JoinedEvents = futureEvents,
                    OtherEvents = notJoinedEvents,
                    PastEvents = pastEvents
                };
                if (volunteer.Role == "Volunteer")
                {
                    return View(viewModel);
                }
                else
                {
                    return View("MyEvents", viewModel);
                }
            }
            else
            {
                return HomePage();
            }
        }
        public class MyEventsViewModel
        {
            public List<OpportunityModel> MyEvents { get; set; }
            public List<OpportunityModel> JoinedEvents { get; set; }
            public List<OpportunityModel> OtherEvents { get; set; }
            public List<OpportunityModel> PastEvents { get; set; }
        }
        [HttpPost]
        public IActionResult Join(EventRegistration model)
        {
            var opportunityDetails = _myDbContext.Opportunities.FirstOrDefault(o => o.Title == model.EventName);
            model.EventEmail = opportunityDetails.UserEmail;
            string userEmail = HttpContext.Session.GetString("email");
            model.UserEmail = userEmail;
            model.Id = Guid.NewGuid();
            model.JoinedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                InsertEventDataIntoDatabase(model);
                return RedirectToAction("Events");
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Leave(EventRegistration model)
        {
            if (model != null)
            {
                string userEmail = HttpContext.Session.GetString("email");
                var existingRegistration = _myDbContext.EventRegistrations
                    .FirstOrDefault(r => r.EventName == model.EventName && r.UserEmail == userEmail);

                if (existingRegistration != null)
                {
                    _myDbContext.EventRegistrations.Remove(existingRegistration);
                    _myDbContext.SaveChanges();
                    return RedirectToAction("Events");
                }
            }
            return View(model);
        }
        private void InsertEventDataIntoDatabase(EventRegistration model)
        {
            try
            {
                _myDbContext.EventRegistrations.Add(model);
                _myDbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "An error occurred while saving your data.");
            }
        }
        private void InsertAlertDataIntoDatabase(Alert model)
        {
            try
            {
                _myDbContext.Alerts.Add(model);
                _myDbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "An error occurred while saving your data.");
            }
        }
        public IActionResult GetDistricts(string province)
        {
            var districts = GetDistrictsByProvince(province);
            return Json(districts);
        }
        public List<District> GetDistrictsByProvince(string province)
        {
            return _myDbContext.Districts
                .Where(d => d.province_id.ToString() == province)
                .OrderBy(d => d.name_en)
                .ToList();
        }
        public IActionResult GetCities(string district)
        {
            var cities = GetCitiesByDistrict(district);
            return Json(cities);
        }
        public List<City> GetCitiesByDistrict(string district)
        {
            return _myDbContext.Cities
                .Where(d => d.district_id.ToString() == district)
                .OrderBy(d => d.name_en)
                .ToList();
        }
        public IActionResult GetProvinces()
        {
            var provinces = _myDbContext.Provinces.OrderBy(p => p.name_en).ToList();
            return Json(provinces);
        }
        public IActionResult Notification()
        {
            string userEmail = HttpContext.Session.GetString("email");
            if (IsUserAuthenticated(userEmail))
            {

                var othervol = _myDbContext.EventRegistrations.Where(p => p.EventEmail == userEmail).ToList();
                var myvol = _myDbContext.EventRegistrations.Where(p => p.UserEmail == userEmail).ToList();

                var allNotifications = othervol.Concat(myvol)
                                                .OrderByDescending(notification => notification.JoinedDate)
                                                .ToList();

                var viewModel = new NotificationViewModel
                {
                    UserEmail = userEmail,
                    AllNotifications = allNotifications
                };

                return View(viewModel);
            }
            return HomePage();
        }
        public class NotificationViewModel
        {
            public string UserEmail { get; set; }
            public List<EventRegistration> AllNotifications { get; set; }
        }
        [HttpPost]
        public IActionResult Update(OpportunityModel updatedModel, string title)
        {
            updatedModel.UserEmail = HttpContext.Session.GetString("email");
            updatedModel.Title = title;
            if (ModelState.IsValid)
            {
                var existingOpportunity = _myDbContext.Opportunities.FirstOrDefault(o => o.Title == updatedModel.Title);

                if (existingOpportunity != null)
                {
                    existingOpportunity.Province = updatedModel.Province;
                    existingOpportunity.District = updatedModel.District;
                    existingOpportunity.City = updatedModel.City;
                    existingOpportunity.Category = updatedModel.Category;
                    existingOpportunity.Date = updatedModel.Date;
                    existingOpportunity.Description = updatedModel.Description;
                    _myDbContext.SaveChanges();
                    return RedirectToAction("Details", new { title = existingOpportunity.Title });
                }
                else
                {
                    return View("OpportunityNotFound");
                }
            }
            return View("Details");
        }
        [HttpPost]
        public IActionResult Edit(string title)
        {
            var opportunityDetails = _myDbContext.Opportunities.FirstOrDefault(o => o.Title == title);
            if (opportunityDetails == null)
            {
                return View("OpportunityNotFound");
            }

            return View(opportunityDetails);
        }
        public IActionResult Delete(string title)
        {
            if (ModelState.IsValid)
            {
                var existingOpportunity = _myDbContext.Opportunities.FirstOrDefault(o => o.Title == title);

                if (existingOpportunity != null)
                {
                    _myDbContext.Opportunities.Remove(existingOpportunity);
                    _myDbContext.SaveChanges();
                    return RedirectToAction("Events");
                }
                else
                {
                    return View("OpportunityNotFound");
                }
            }
            return View("Events");
        }
        public IActionResult ViewAlerts()
        {
            List<Alert> locAlerts = new List<Alert>();
            List<Alert> bTypeAlerts = new List<Alert>();
            List<Alert> teamAlerts = new List<Alert>();
            List<Alert> myAlerts = new List<Alert>();
            string user = HttpContext.Session.GetString("email");
            VolunteerRegister volunteer = _myDbContext.Volunteers.FirstOrDefault(v => v.Email == user);
            if (volunteer != null && volunteer.Role == "Volunteer")
            {
                var info = _myDbContext.AlertInfo.FirstOrDefault(o => o.Email == user);
                var alerts = _myDbContext.Alerts.Where(o => o.UserEmail != user).ToList();

                foreach (var alert in alerts)
                {
                    switch (alert.Location)
                    {
                        case "City":
                            if (alert.City != null && alert.City == info?.City)
                            {
                                locAlerts.Add(alert);
                            }
                            break;
                        case "Province":
                            if (alert.Province != null && alert.Province == info?.Province)
                            {
                                locAlerts.Add(alert);
                            }
                            break;
                        case "District":
                            if (alert.District != null && alert.District == info?.District)
                            {
                                locAlerts.Add(alert);
                            }
                            break;
                    }

                    if (alert.BloodGroup != null && alert.BloodGroup == info?.BloodGroup)
                    {
                        bTypeAlerts.Add(alert);
                    }

                    if (alert.Team != null && alert.Team == info?.Team)
                    {
                        teamAlerts.Add(alert);
                    }
                }

                List<Alert> sortedLocAlerts = locAlerts.OrderBy(a => a.DueDate).ToList();
                List<Alert> sortedbTypeAlerts = bTypeAlerts.OrderBy(a => a.DueDate).ToList();
                List<Alert> sortedteamAlerts = teamAlerts.OrderBy(a => a.DueDate).ToList();

                var viewModel = new MyAlertViewModel
                {
                    LocAlerts = sortedLocAlerts,
                    BTypeAlerts = sortedbTypeAlerts,
                    TeamAlerts = sortedteamAlerts
                };

                return View(viewModel);
            }
            else
            {
                var alert = _myDbContext.Alerts.FirstOrDefault(o => o.UserEmail == user);
                if (alert == null)
                {
                    return View("OpportunityNotFound");
                }
                else if (user != null && alert.UserEmail == user)
                {
                    myAlerts.Add(alert);
                    List<Alert> sortedAlerts = myAlerts.OrderBy(a => a.DueDate).ToList();
                    return View("MyAlerts", sortedAlerts);
                }
                return RedirectToAction("HomePage");
            }
        }


        public class MyAlertViewModel
        {
            public List<Alert> LocAlerts { get; set; }
            public List<Alert> BTypeAlerts { get; set; }
            public List<Alert> TeamAlerts { get; set; }
        }
        [HttpPost]
        public IActionResult RateEvent(RateModel model)
        {
            model.UserEmail = HttpContext.Session.GetString("email");
            model.Id = Guid.NewGuid();
            model.Date= DateTime.Now.Date;
            var name = model.EventName;
            if (ModelState.IsValid)
            {
                var existingRating = _myDbContext.EventRating.FirstOrDefault(u => u.UserEmail == model.UserEmail);

                if (existingRating == null)
                {
                    _myDbContext.EventRating.Add(model);
                }
                else
                {
                    existingRating.Rating = model.Rating;
                    _myDbContext.Entry(existingRating).State = EntityState.Modified;
                }
                _myDbContext.SaveChanges();

                return RedirectToAction("PastEvent", "Opportunity", new { title = name });
            }
            return View(model);
        }
        public IActionResult AlertDetails(string title)
        {
            string userEmail = HttpContext.Session.GetString("email");
            if (IsUserAuthenticated(userEmail))
            {
                var opportunityDetails = _myDbContext.Alerts.FirstOrDefault(o => o.Title == title);
                if (opportunityDetails == null)
                {
                    return View("OpportunityNotFound");
                }

                return View(opportunityDetails);
            }
            else
            {
                return HomePage();
            }
        }
    }
}
