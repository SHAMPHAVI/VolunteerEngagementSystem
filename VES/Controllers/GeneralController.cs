using Microsoft.AspNetCore.Mvc;
using VES.Data;
using VES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace VES.Controllers
{
    public class GeneralController : Controller
    {
        private readonly MyDbContext _myDbContext;

        public GeneralController(MyDbContext context)
        {
            _myDbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ActionResult GetStarted()
        {
            return View();
        }

        public IActionResult Calendar(int? year, int? month)
        {
            DateTime today = DateTime.Now.Date;
            string userEmail = HttpContext.Session.GetString("email");
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            if (year.HasValue && month.HasValue)
            {
                currentYear = year.Value;
                currentMonth = month.Value;
            }
            int previousMonth = currentMonth - 1;
            int nextMonth = currentMonth + 1;
            if (previousMonth < 1)
            {
                previousMonth = 12;
                currentYear--;
            }

            if (nextMonth > 12)
            {
                nextMonth = 1;
                currentYear++;
            }
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
                PastEvents = pastEvents,
                Year = currentYear,
                PreviousMonth = previousMonth,
                NextMonth = nextMonth,
                Month= currentMonth
            };

            if (volunteer?.Role == "Volunteer")
            {
                return View(viewModel);
            }
            else
            {
                return View("MyEvents", viewModel);
            }
        }

        public class MyEventsViewModel
        {
            //public MyEventsViewModel()
            //{
            //    MyEvents = new List<OpportunityModel>();
            //    JoinedEvents = new List<OpportunityModel>();
            //    OtherEvents = new List<OpportunityModel>();
            //    PastEvents = new List<OpportunityModel>();
            //}

            public List<OpportunityModel> MyEvents { get; set; }
            public List<OpportunityModel> JoinedEvents { get; set; }
            public List<OpportunityModel> OtherEvents { get; set; }
            public List<OpportunityModel> PastEvents { get; set; }
            public int Year { get; set; }
            public int PreviousMonth { get; set; }
            public int NextMonth { get; set; }
            public int Month { get; set; }
            public IEnumerable<CalendarWeek> GetCalendarWeeks(int year, int month)
            {
                List<CalendarWeek> calendarWeeks = new List<CalendarWeek>();
                int daysInMonth = DateTime.DaysInMonth(year, month);

                for (int day = 1; day <= daysInMonth; day++)
                {
                    DateTime currentDate = new DateTime(year, month, day);

                    if (currentDate.DayOfWeek == DayOfWeek.Sunday || day == 1)
                    {
                        CalendarWeek week = new CalendarWeek();

                        for (int i = 0; i < 7 && day + i <= daysInMonth; i++)
                        {
                            CalendarDay calendarDay = new CalendarDay
                            {
                                Day = day + i,
                                Title = GetEventsForDay(year, month, day)
                            };

                            week.Days.Add(calendarDay);
                        }

                        calendarWeeks.Add(week);
                    }
                }

                return calendarWeeks;
            }

            public IEnumerable<OpportunityModel> GetEventsForDate(DateTime date)
            {
                return MyEvents
                    .Concat(JoinedEvents)
                    .Concat(OtherEvents)
                    .Concat(PastEvents)
                    .Where(evt => evt.Date.Date == date.Date);
            }

            public IEnumerable<OpportunityModel> GetEventsForDay(int year, int month, int day)
            {
                return MyEvents
                    .Concat(JoinedEvents)
                    .Concat(OtherEvents)
                    .Concat(PastEvents)
                    .Where(o => o.Date.Year == year && o.Date.Month == month && o.Date.Day == day);
            }
        }

        public class CalendarWeek
        {
            public List<CalendarDay> Days { get; set; } = new List<CalendarDay>();
        }

        public class CalendarDay
        {
            public int Day { get; set; }
            public IEnumerable<OpportunityModel> Title { get; set; } = new List<OpportunityModel>();
        }
    }
}
