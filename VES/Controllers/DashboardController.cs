using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Drawing;
using VES.Data;
using VES.Models;
using static VES.Controllers.DashboardController;

namespace VES.Controllers
{
    public class DashboardController : Controller
    {
        private MyDbContext _myDbContext;
        public DashboardController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }
        public ActionResult Users()
        {
            string userEmail = HttpContext.Session.GetString("email");
            bool isAdmin = _myDbContext.Admin.Any(a => a.Email == userEmail);

            if (isAdmin)
            {
                var volunteers = _myDbContext.Volunteers.ToList();
                var role = volunteers.GroupBy(v => v.Role)
                    .ToDictionary(group => group.Key, group => group.ToList());
                var gender = volunteers.Where(v=>v.Role=="Volunteer").GroupBy(v => v.Category)
                    .ToDictionary(group => group.Key, group => group.ToList());
                var category = volunteers.Where(v => v.Role == "Organization").GroupBy(v => v.Category)
                    .ToDictionary(group => group.Key, group => group.ToList());
                var model = new UsersViewModel
                {
                    Category = category,
                    Role = role,
                    Gender = gender
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public class UsersViewModel
        {
            public Dictionary<string, List<VolunteerRegister>> Role { get; set; }
            public Dictionary<string, List<VolunteerRegister>> Gender { get; set; }
            public Dictionary<string, List<VolunteerRegister>> Category { get; set; }
        }
        public ActionResult Events()
        {
            string userEmail = HttpContext.Session.GetString("email");
            bool isAdmin = _myDbContext.Admin.Any(a => a.Email == userEmail);

            if (isAdmin)
            {
                var events = _myDbContext.Opportunities.ToList();
                var category = events.GroupBy(v => v.Category)
                    .ToDictionary(group => group.Key, group => group.ToList());
                var district = events.GroupBy(v => v.District)
                    .ToDictionary(group => group.Key, group => group.ToList());
                var city = events.GroupBy(v => v.City)
                    .ToDictionary(group => group.Key, group => group.ToList());
                var province = events.GroupBy(v => v.Province)
                    .ToDictionary(group => group.Key, group => group.ToList());
                var model = new OpportunityViewModel
                {
                    Category = category,
                    Province = province,
                    District = district,
                    City = city
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public class OpportunityViewModel
        {
            public Dictionary<string, List<OpportunityModel>> Category { get; set; }
            public Dictionary<string, List<OpportunityModel>> Province { get; set; }
            public Dictionary<string, List<OpportunityModel>> District { get; set; }
            public Dictionary<string, List<OpportunityModel>> City { get; set; }
        }
      
        public ActionResult Ratings()
        {
            string userEmail = HttpContext.Session.GetString("email");
            bool isAdmin = _myDbContext.Admin.Any(a => a.Email == userEmail);
            if (isAdmin)
            {
                var model = Rate();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public RatingViewModel Rate()
        {
            var prating = _myDbContext.ParticipantRating.ToList();
            var erating = _myDbContext.EventRating.ToList();

            var participantRatings = prating.GroupBy(v => v.Participant).ToDictionary(group => group.Key, group => group.ToList());
            var eventRatings = erating.GroupBy(v => v.EventEmail).ToDictionary(group => group.Key, group => group.ToList());

            var VRating = CalculateAverageRating(participantRatings);
            var ORating = CalculateAverageRating(eventRatings);
            var averageVolRating = CalculateAvgRating(prating, erating, "Volunteer");
            var averageOrgRating = CalculateAvgRating(prating, erating, "Organization");

            var model = new RatingViewModel
            {
                ParticipantRating = VRating,
                EventRating = ORating,
                VolRating = averageVolRating,
                OrgRating = averageOrgRating
            };

            return model;
        } 
        private Dictionary<string, double> CalculateAvgRating( List<ParticipantRating>pRatings, List<RateModel> eRatings, string role)
        {
            var result = new Dictionary<string, double>();
            var users = _myDbContext.Volunteers.Where(o => o.Role == role).ToList();
            
            foreach ( var o in users )
            {
                var UserId = o.Email;
                var Count=1;
                double overallAvgRating = 0;
                var pRating = pRatings.Where(o=>o.Participant==UserId).Select(r => r.Rating);
                var eRating = eRatings.Where(o => o.EventEmail == UserId).Select(r => r.Rating);
                var pAvgRating = pRating.Sum();
                var eAvgRating = eRating.Sum();
                if (pAvgRating != 0 && eAvgRating != 0) { Count=pRating.Count()+eRating.Count();
                    overallAvgRating = (float)((pAvgRating + eAvgRating) / Count);
                }
                else if (pAvgRating != 0 ) { Count = pRating.Count(); overallAvgRating = (double)pAvgRating / (double)Count; }
                else if (eAvgRating != 0 ) { Count = eRating.Count(); overallAvgRating = (double)eAvgRating / (double)Count; } 
                
                result.Add(UserId, overallAvgRating);

            }

            return result;
        }
        private Dictionary<string, double> CalculateAverageRating<TKey, TValue>(Dictionary<TKey, List<TValue>> ratings)
            where TValue : class
        {
            var result = new Dictionary<string, double>();

            foreach (var ratingGroup in ratings.Where(pair => pair.Value.Any()))
            {
                double? averageRating = ratingGroup.Value.Average(r => GetRatingValue(r));
                result.Add(ratingGroup.Key.ToString(), averageRating ?? 0);
            }

            return result;
        }
        private double GetRatingValue(object rating)
        {

            if (rating is ParticipantRating participantRating)
            {
                return (double)participantRating.Rating;
            }
            else if (rating is RateModel ERating)
            {
                return (double)ERating.Rating;
            }
            return 0;
        }

        public class Rating
        {
            public double RatingValue { get; set; }
            public string email { get; set; }
        }
        public class RatingViewModel
        {
            public Dictionary<string, double> EventRating { get; set; }
            public Dictionary<string, double> ParticipantRating { get; internal set; }
            public Dictionary<string, double> VolRating { get; set; }
            public Dictionary<string, double> OrgRating { get; set; }
        }
        public class EngagementModel
        {
            public Dictionary<string, int> Participant { get; set; }
            public Dictionary<string, int> Organizer { get; set; }
            public Dictionary<string, int> Event { get; set; }
        }
        public EngagementModel EngagementCount()
        {
            var reg = _myDbContext.EventRegistrations.ToList();
            var vol = reg.GroupBy(v => v.UserEmail).ToDictionary(group => group.Key, group => group.Count());
            var org = reg.GroupBy(v => v.EventEmail).ToDictionary(group => group.Key, group => group.Count());
            var name = reg.GroupBy(v => v.EventName).ToDictionary(group => group.Key, group => group.Count());
            var categoryCounts = new Dictionary<string, int>();

            foreach (var entry in name)
            {
                string eventName = entry.Key;
                int count = entry.Value;
                string category = _myDbContext.Opportunities
                                               .Where(e => e.Title == eventName)
                                               .Select(e => e.Category)
                                               .FirstOrDefault();
                if (!string.IsNullOrEmpty(category))
                {
                    if (categoryCounts.ContainsKey(category))
                    {
                        categoryCounts[category] += count;
                    }
                    else
                    {
                        categoryCounts.Add(category, count);
                    }
                }
            }
            var model = new EngagementModel
            {
                Participant = vol,
                Organizer = org,
                Event = categoryCounts
            };
            return model;
        }
        public ActionResult Engagement()
        {
            string userEmail = HttpContext.Session.GetString("email");
            bool isAdmin = _myDbContext.Admin.Any(a => a.Email == userEmail);
            if (isAdmin)
            {
                var model = EngagementCount();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Volunteers()
        {
            string userEmail = HttpContext.Session.GetString("email");
            bool isAdmin = _myDbContext.Admin.Any(a => a.Email == userEmail);
            var reg = _myDbContext.AlertInfo.ToList();
            var vol = reg.GroupBy(v => v.Team ?? "NIL").ToDictionary(group => group.Key, group => group.ToList());
            var org = reg.GroupBy(v => v.BloodGroup ?? "NIL").ToDictionary(group => group.Key, group => group.ToList());
            var name = reg.GroupBy(v => v.Province ?? "NIL").ToDictionary(group => group.Key, group => group.ToList());
            if (isAdmin)
            {
                var model = new AlertViewModel
                {
                    Team = vol,
                    Blood = org,
                    Province = name
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public class AlertViewModel
        {
            public Dictionary<string, List<AlertModel>> Team { get; set; }
            public Dictionary<string, List<AlertModel>> Blood { get; set; }
            public Dictionary<string, List<AlertModel>> Province { get; set; }
        }
        public ActionResult Alerts()
        {
            string userEmail = HttpContext.Session.GetString("email");
            bool isAdmin = _myDbContext.Admin.Any(a => a.Email == userEmail);
            var reg = _myDbContext.Alerts.ToList();
            var team = reg.GroupBy(v => v.Team ?? "NIL").ToDictionary(group => group.Key, group => group.ToList());
            var blood = reg.GroupBy(v => v.BloodGroup ?? "NIL").ToDictionary(group => group.Key, group => group.ToList());
            var province = reg.Where(v => v.Location=="Province").GroupBy(v => v.Province ?? "NIL").ToDictionary(group => group.Key, group => group.ToList());
            var district = reg.Where(v => v.Location=="District").GroupBy(v => v.District ?? "NIL").ToDictionary(group => group.Key, group => group.ToList());
            var city = reg.Where(v => v.Location=="City").GroupBy(v => v.City ?? "NIL").ToDictionary(group => group.Key, group => group.ToList());
            if (isAdmin)
            {
                var model = new PostAlertModel
                {
                    Team = team,
                    Blood = blood,
                    Province = province,
                    District = district,
                    City = city,
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public class PostAlertModel
        {
            public Dictionary<string, List<Alert>> Team { get; set; }
            public Dictionary<string, List<Alert>> Blood { get; set; }
            public Dictionary<string, List<Alert>> Province { get; set; }
            public Dictionary<string, List<Alert>> City { get; set; }
            public Dictionary<string, List<Alert>> District { get; set; }
        }
        public ActionResult Overview()
        {
            string userEmail = HttpContext.Session.GetString("email");
            bool isAdmin = _myDbContext.Admin.Any(a => a.Email == userEmail);
            var users = _myDbContext.Volunteers.ToList();
            var events = _myDbContext.Opportunities.ToList();
            var alerts = _myDbContext.Alerts.ToList();
            if (isAdmin)
            {
                var model = new PostOverviewModel
                {
                    Users = users,
                    Events = events,
                    Alerts = alerts,
                    Engagement = EngagementCount(),
                    Rating = Rate()
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public class PostOverviewModel
        {
            public List<VolunteerRegister> Users { get; set; }
            public List<OpportunityModel> Events { get; set; }
            public List<Alert> Alerts { get; set; }
            public EngagementModel  Engagement { get; set; }
            public RatingViewModel Rating { get; set; }
        }
    }
}
