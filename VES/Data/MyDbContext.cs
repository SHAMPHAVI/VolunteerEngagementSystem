using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VES.Models;
using VES.Models.Admin;
using VES.Models.Location;

namespace VES.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        public DbSet<VolunteerRegister> Volunteers { get; set; }
        public DbSet<AdminLogin> Admin { get; set; }
        public DbSet<OpportunityModel> Opportunities { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<AlertModel> AlertInfo { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<RateModel> EventRating { get; set; }
        public DbSet<ParticipantRating> ParticipantRating { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }
        public DbSet<EventPhotos> Images { get; set; }

    }
}
