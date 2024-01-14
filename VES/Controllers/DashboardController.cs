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

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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

    }
}
