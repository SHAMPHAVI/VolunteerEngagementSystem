using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VES.Data;
using VES.Models.Volunteer;
using Org.BouncyCastle.Crypto.Generators;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        [HttpPost]
        public IActionResult Add(OpportunityModel model)
        {
            string userEmail = HttpContext.Session.GetString("email");
            model.UserEmail = userEmail;
            if (ModelState.IsValid)
            {
                InsertOpportunityDataIntoDatabase(model);
                return RedirectToAction("ViewAll");
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

    }
}
