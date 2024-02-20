using Microsoft.AspNetCore.Mvc;
using VES.Data;
using VES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VES.Controllers
{
    public class GeneralController : Controller
    {
        private readonly MyDbContext _myDbContext;

        public GeneralController(MyDbContext context)
        {
            _myDbContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        public IActionResult Chat(string title)
        {
            var comments = _myDbContext.Comments.Where(c => c.Title == title).ToList();
           ViewBag.Title = title;
            return View(comments);
        }
        [HttpPost]
        public ActionResult AddComment(string newCommentText, string replyto, string type, string title)
        {
            string userEmail = HttpContext.Session.GetString("email");
            var newComment = new CommentModel
            {
                Id = Guid.NewGuid(),
                UserEmail = userEmail, 
                CommentText = newCommentText,
                Timestamp = DateTime.Now,
                Title = title, 
                type = type,
                replyto = replyto
            };

            _myDbContext.Comments.Add(newComment);
            _myDbContext.SaveChanges(); 

            
            return RedirectToAction("Chat", new { title = newComment.Title });
        }
        public ActionResult GetStarted()
        {
            return View();
        }
        private bool IsUserAuthenticated(string userEmail)
        {
            return _myDbContext.Volunteers.Any(a => a.Email == userEmail);
        }
        private RedirectToActionResult HomePage()
        {
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Calendar(int? year, int? month)
        {
            string userEmail = HttpContext.Session.GetString("email");
            if (IsUserAuthenticated(userEmail))
            {
            DateTime today = DateTime.Now.Date;
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
                var myEvents = new List<EventData>
                {
                    new EventData
                    {
                        Events = _myDbContext.Opportunities.Where(o => o.UserEmail == userEmail).ToList(),
                        Color = "My Event"
                    }
                };
                VolunteerRegister volunteer = _myDbContext.Volunteers.FirstOrDefault(v => v.Email == userEmail);
            List<OpportunityModel> joinedEvents = new List<OpportunityModel>();
            var regEvents = _myDbContext.EventRegistrations.Where(o => o.UserEmail == userEmail).ToList();

            foreach (var reg in regEvents)
            {
                var title = reg.EventName;
                var events = _myDbContext.Opportunities.Where(o => o.Title == title);
                joinedEvents.AddRange(events);
            }

                var pastEvents = new List<EventData>
                {
                    new EventData
                    {
                        Events = joinedEvents.Where(o => o.Date < today).ToList(),
                        Color = "Past Joined Event"
                    }
                };
              var futureEvents = new List<EventData>
                {
                    new EventData
                    {
                        Events = joinedEvents.Where(o => o.Date >= today).ToList(),
                        Color = "Future Joined Event"
                    }
                };
                var othersEvents = _myDbContext.Opportunities.Where(o => o.UserEmail != userEmail).ToList();
                var notJoinedEvents = new List<EventData>
                {
                    new EventData
                    {
                        Events = othersEvents.Where(o => !joinedEvents.Any(j => j.Title == o.Title)).ToList(),
                        Color = "Not Joined Event"
                    }
                };

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

                return View(viewModel);

            }
            else
            {
                return HomePage();
            }
        }
        public class EventData
        {
            public List<OpportunityModel> Events { get; set; }
            public string Color { get; set; }
        }
        public class MyEventsViewModel
        {
            public List<EventData> MyEvents { get; set; }
            public List<EventData> JoinedEvents { get; set; }
            public List<EventData> OtherEvents { get; set; }
            public List<EventData> PastEvents { get; set; }
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

            public IEnumerable<EventData> GetEventsForDate(DateTime date)
            {
                return MyEvents
                    .Concat(JoinedEvents)
                    .Concat(OtherEvents)
                    .Concat(PastEvents)
                   .Where(evt => evt.Events.Any(e => e.Date.Date == date.Date));
            }
            public IEnumerable<EventData> GetEventsForDay(int year, int month, int day)
            {
                return MyEvents
                    .Concat(JoinedEvents)
                .Concat(OtherEvents)
                .Concat(PastEvents)
                 .Where(o => o.Events.Any(e => e.Date.Year == year && e.Date.Month == month && e.Date.Day == day));
            }

        }

        public class CalendarWeek
        {
            public List<CalendarDay> Days { get; set; } = new List<CalendarDay>();
        }

        public class CalendarDay
        {
            public int Day { get; set; }
            public IEnumerable<EventData> Title { get; set; } = new List<EventData>();
        }
    }
}
