using ClientCommon.Data;
using System.Data.Entity;

namespace Server.Access
{
    public class AccessDB : DbContext
    {
        public AccessDB() : base("EmployeeServiceDB") { }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Team> Teams { get; set; }
    }

}
