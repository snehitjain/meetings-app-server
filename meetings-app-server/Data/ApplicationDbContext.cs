using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using meetings_app_server.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace meetings_app_server.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>  // Inherit from IdentityDbContext
    {
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Attendee> Attendees { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

      

        // Configure model relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Meeting data
            var meetings = new List<Meeting>
            {
                new Meeting
                {
                    Id = 1,
                    Name = "Google marketing campaign",
                    Description = "Increasing brand awareness and spreading information about new products",
                    Date = new DateTime(2020, 10, 28),
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(10, 30)
                }
            };

            modelBuilder.Entity<Meeting>().HasData(meetings);

            // Define the many-to-many relationship between Meeting and Attendee
            modelBuilder.Entity<Attendee>()
                .HasKey(a => new { a.MeetingId, a.UserId });  // Composite key

            modelBuilder.Entity<Attendee>()
                .HasOne(a => a.Meeting)
                .WithMany(m => m.Attendees)
                .HasForeignKey(a => a.MeetingId);

            modelBuilder.Entity<Attendee>()
                .HasOne(a => a.User)
                .WithMany() // One user can attend many meetings
                .HasForeignKey(a => a.UserId);
        }
    }
}
