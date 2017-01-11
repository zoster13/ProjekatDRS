using System.Data.Entity.Migrations;

namespace Server.Access
{
    public class Configuration : DbMigrationsConfiguration<AccessDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "EmployeeServiceDB";
        }

    }
}
