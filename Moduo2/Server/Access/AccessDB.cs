using ClientCommon.Data;
using System.Data.Entity;

namespace Server.Access
{
    public class AccessDB : DbContext
    {
        public AccessDB() : base("EmployeeServiceDB") { }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<HiringCompany> HiringCompanies { get; set; }

        public DbSet<UserStory> UserStories { get; set; }
    }
}
