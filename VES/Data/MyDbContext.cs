using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using VES.Models.Volunteer;

namespace VES.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }


        public DbSet<VolunteerRegister> Users { get; set; }
    }
}
