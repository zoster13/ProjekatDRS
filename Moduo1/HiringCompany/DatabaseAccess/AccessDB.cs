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

        public DbSet<Employee> employees { get; set; }
        public DbSet<Project> projects { get; set; }
        public DbSet<PartnerCompany> companies { get; set; }
        //public DbSet<UserStory> userstories { get; set; }
    
    
    }
}
