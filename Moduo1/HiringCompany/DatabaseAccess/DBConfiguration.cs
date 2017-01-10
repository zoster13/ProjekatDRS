using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany.DatabaseAccess
{
    public class DBConfiguration : DbMigrationsConfiguration<AccessDB>
    {
        public DBConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "ServiceDB"; // name of .mdf database file
        }
    }
}
