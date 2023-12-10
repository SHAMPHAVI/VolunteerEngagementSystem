using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VES.Data;
using VES.Models;
using Microsoft.AspNetCore.Authentication;
using VES.Models.Location;

namespace VES.Controllers
{
    public class OpportunityController : Controller
    {
        private MyDbContext _myDbContext;
        public OpportunityController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }
       
        public IActionResult Add()
        {
            return View();
        }
      
        public IActionResult Details(string title)
        {
            var opportunityDetails = _myDbContext.Opportunities.FirstOrDefault(o => o.Title == title);
            if (opportunityDetails == null)
            {
                return View("OpportunityNotFound");
            }

            return View(opportunityDetails);
        }
        public IActionResult JoinEvents(string title)
        {
            var opportunityDetails = _myDbContext.Opportunities.FirstOrDefault(o => o.Title == title);
            if (opportunityDetails == null)
            {
                return View("OpportunityNotFound");
            }

            return View(opportunityDetails);
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
        public IActionResult ViewAll()
        {
            var opportunities = _myDbContext.Opportunities.ToList();
            return View(opportunities);
        }
        public IActionResult Events()
        {
            List<OpportunityModel> joinedEvents = new List<OpportunityModel>();
            string userEmail = HttpContext.Session.GetString("email");
            var myEvents = _myDbContext.Opportunities.Where(o => o.UserEmail == userEmail).ToList();
            var regEvents = _myDbContext.EventRegistrations.Where(o => o.UserEmail == userEmail).ToList();
            foreach (var reg in regEvents) {
                var title = reg.EventName;
                var events = _myDbContext.Opportunities.Where(o => o.Title == title);
               joinedEvents.AddRange(events);
            }
            var othersEvents = _myDbContext.Opportunities.Where(o => o.UserEmail != userEmail).ToList();
            var notJoinedEvents = othersEvents.Where(o => !joinedEvents.Any(j => j.Title == o.Title)).ToList();
            var viewModel = new MyEventsViewModel
            {
                MyEvents = myEvents,
                JoinedEvents = joinedEvents,
                OtherEvents= notJoinedEvents
            };
            return View(viewModel);
        }
        public class MyEventsViewModel
        {
            public List<OpportunityModel> MyEvents { get; set; }
            public List<OpportunityModel> JoinedEvents { get; set; }
            public List<OpportunityModel> OtherEvents { get; set; }
        }
        [HttpPost]
        public IActionResult Join(EventRegistration model)
        {
            var opportunityDetails = _myDbContext.Opportunities.FirstOrDefault(o => o.Title == model.EventName);
            model.EventEmail = opportunityDetails.UserEmail;
            string userEmail = HttpContext.Session.GetString("email");
            model.UserEmail = userEmail;
            model.Id= Guid.NewGuid();
            if (ModelState.IsValid)
            {
                InsertEventDataIntoDatabase(model);
                return RedirectToAction("Events");
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
    }
}
