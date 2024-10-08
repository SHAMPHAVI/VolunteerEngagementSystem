﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using VES.Data;
using VES.Models;
using static VES.Controllers.GeneralController;
namespace VES.Controllers
{
    public class GeneralController : Controller
    {
        private readonly MyDbContext _myDbContext;
        private readonly IWebHostEnvironment _environment;

        public GeneralController(MyDbContext context, IWebHostEnvironment environment)
        {
            _myDbContext = context ?? throw new ArgumentNullException(nameof(context));
            _environment = environment;
        }
        [HttpPost]
        public IActionResult UploadImage(IFormFile imageFile, string eventTitle)
        {
            string userEmail = HttpContext.Session.GetString("email");
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "resources");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                Guid id = Guid.NewGuid();
                string title = eventTitle.Replace(" ", "");
                string uniqueFileName = id.ToString() + "_" + title + ".jpg";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var image = Image.FromStream(imageFile.OpenReadStream()))
                {
                    int maxWidth = 1500;
                    int maxHeight = 1500; 

                    int newWidth = image.Width;
                    int newHeight = image.Height;

                    if (image.Width > maxWidth)
                    {
                        newWidth = maxWidth;
                        newHeight = (int)Math.Round((double)image.Height * maxWidth / image.Width);
                    }

                    if (newHeight > maxHeight)
                    {
                        newHeight = maxHeight;
                        newWidth = (int)Math.Round((double)image.Width * maxHeight / image.Height);
                    }

                    using (var resizedImage = new Bitmap(newWidth, newHeight))
                    {
                        using (var graphics = Graphics.FromImage(resizedImage))
                        {
                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                        }

                        resizedImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }

                var imageModel = new EventPhotos
                {
                    Id = Guid.NewGuid(),
                    UserEmail = userEmail,
                    ImagePath = uniqueFileName,
                    Timestamp = DateTime.Now,
                    Title = eventTitle
                };

                _myDbContext.Images.Add(imageModel);
                _myDbContext.SaveChanges();

                return RedirectToAction("EventPhotos", new { title = eventTitle });
            }

            return RedirectToAction("EventPhotos", new { title = eventTitle });
        }
        public IActionResult EventPhotos(string title)
        {
            string eventTitle = title;
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "resources");
            string[] imageFiles = Directory.GetFiles(uploadsFolder, "*.jpg");
            if(title!=null)
            {
                eventTitle = title.Replace(" ", "");

            }
            var imagesWithTitle = imageFiles.Where(file => !string.IsNullOrEmpty(title) && Path.GetFileNameWithoutExtension(file).Contains(eventTitle)).Select(Path.GetFileName);
            ViewBag.Title = title;
            return View(imagesWithTitle.ToList());
        }
        public IActionResult Chat(string title)
        {
            var comments = _myDbContext.Comments.Where(c => c.Title == title).OrderByDescending(c => c.Timestamp).ToList();
           ViewBag.Title = title;
            return View(comments);
        }
        public IActionResult Reviews(string title)
        {
            var comments = _myDbContext.Reviews.Where(c => c.Title == title).OrderByDescending(c => c.Timestamp).ToList();
            ViewBag.Title = title;
            return View(comments);
        }
        
        public IActionResult ReviewReplies(Guid commentId)
        {

            var mainComment = _myDbContext.Reviews.FirstOrDefault(c => c.Id == commentId);
            ViewBag.Title = mainComment.Title;
            var replies = _myDbContext.Reviews.Where(c => c.type == "reply" && c.replyto == commentId.ToString()).OrderByDescending(c => c.Timestamp).ToList();

            var model = new ReviewPageViewModel
            {
                MainComment = mainComment,
                Replies = replies
            };

            return View(model);
        }
        public IActionResult ReplyPage(Guid commentId)
        {
           
            var mainComment = _myDbContext.Comments.FirstOrDefault(c => c.Id == commentId);
            ViewBag.Title = mainComment.Title;
            var replies = _myDbContext.Comments.Where(c => c.type == "reply" && c.replyto == commentId.ToString()).OrderByDescending(c => c.Timestamp).ToList();

            var model = new ReplyPageViewModel
            {
                MainComment = mainComment,
                Replies = replies
            };

            return View(model);
        }
        public class ReviewPageViewModel
        {
            public ReviewModel MainComment { get; set; }
            public List<ReviewModel> Replies { get; set; }
        }
        public class ReplyPageViewModel
        {
            public CommentModel MainComment { get; set; }
            public List<CommentModel> Replies { get; set; }
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
        [HttpPost]
        public ActionResult AddReview(string newCommentText, string replyto, string type, string title)
        {
            string userEmail = HttpContext.Session.GetString("email");
            var newComment = new ReviewModel
            {
                Id = Guid.NewGuid(),
                UserEmail = userEmail,
                CommentText = newCommentText,
                Timestamp = DateTime.Now,
                Title = title,
                type = type,
                replyto = replyto
            };

            _myDbContext.Reviews.Add(newComment);
            _myDbContext.SaveChanges();


            return RedirectToAction("Reviews", new { title = newComment.Title });
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
                Month= currentMonth
            };

                if (volunteer.Role == "Volunteer")
                {
                    return View(viewModel);
                }
                else
                {
                    return View("MyCalendar", viewModel);
                }

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
            public int Month { get; set; }
            List<CalendarWeek> calendarWeeks = new List<CalendarWeek>();
            public IEnumerable<CalendarWeek> GetCalendarWeeks(int year, int month)
            {

                int daysInMonth = DateTime.DaysInMonth(year, month);

                for (int day = 1; day <= daysInMonth; day++)
                {
                    DateTime currentDate = new DateTime(year, month, day);

                    if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        CalendarWeek week = new CalendarWeek();

                        for (int i = 0; i < 7 && day + i <= daysInMonth; i++)
                        {
                            CalendarDay calendarDay = new CalendarDay
                            {
                                Day = day + i,
                            };

                            week.Days.Add(calendarDay);
                        }

                        calendarWeeks.Add(week);
                    }
                    else if (day == 1)
                    {
                        var w = currentDate.DayOfWeek;
                        int d = 0;
                        int i = 0;
                        switch (w)
                        {
                            case DayOfWeek.Monday: d = 1; break;
                            case DayOfWeek.Tuesday: d = 2; break;
                            case DayOfWeek.Wednesday: d = 3; break;
                            case DayOfWeek.Thursday: d = 4; break;
                            case DayOfWeek.Friday: d = 5; break;
                            case DayOfWeek.Saturday: d = 6; break;

                        }
                        CalendarWeek week = new CalendarWeek();
                        for (i = 0; i < 7; i++)

                        {
                            if (i < d)
                            {
                                week.Days.Add(null);
                            }
                            else
                            {
                                CalendarDay calendarDay = new CalendarDay
                                {
                                    Day = day+i-d,
                                };

                                week.Days.Add(calendarDay);
                            }
                        }
                        calendarWeeks.Add(week);
                       
                    }
                }

                return calendarWeeks;
            }

            public IEnumerable<EventData> GetMyEventsForDate(DateTime date)
            {

                DateTime startOfDay = date.Date;
                DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                var myEvents = MyEvents.Where(e => e.Events.Any(eventModel => eventModel.Date >= startOfDay && eventModel.Date <= endOfDay));


                return myEvents;
            }
            public IEnumerable<EventData> GetEventsForDate(DateTime date)
            {
                DateTime startOfDay = date.Date;
                DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);
                var jEvents = JoinedEvents.Where(e => e.Events.Any(eventModel => eventModel.Date >= startOfDay && eventModel.Date <= endOfDay));
                var myEvents = MyEvents.Where(e => e.Events.Any(eventModel => eventModel.Date >= startOfDay && eventModel.Date <= endOfDay));
                var oEvents = OtherEvents.Where(e => e.Events.Any(eventModel => eventModel.Date >= startOfDay && eventModel.Date <= endOfDay));
                var pEvents = PastEvents.Where(e => e.Events.Any(eventModel => eventModel.Date >= startOfDay && eventModel.Date <= endOfDay));
                var eventsForDate = myEvents
                    .Concat(jEvents)
                    .Concat(oEvents)
                    .Concat(pEvents);

                return eventsForDate;
            }

        }

        public class CalendarWeek
        {
            public List<CalendarDay> Days { get; set; } = new List<CalendarDay>();
        }

        public class CalendarDay
        {
            public int Day { get; set; }
        }
     
    }
}
