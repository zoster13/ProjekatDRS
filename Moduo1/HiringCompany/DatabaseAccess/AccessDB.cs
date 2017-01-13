using EmployeeCommon;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany.DatabaseAccess
{
    public class AccessDB : DbContext
    {
        public AccessDB() : base("hiringCompanyDB") { } // hiringCompanyDB -> Database name

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<PartnerCompany> Companies { get; set; }
        //public DbSet<UserStory> userstories { get; set; }   
    }
}
