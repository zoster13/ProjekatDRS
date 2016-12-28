using System.Data.Entity.Migrations;

namespace Server.Access
{
    class Configuration : DbMigrationsConfiguration<AccessDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "EmployeeServiceDB";
        }

    }
}
