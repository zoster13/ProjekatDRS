using ClientCommon.Data;
using Server.Access;
using System.Collections.Generic;
using System.Linq;

namespace Server.Database
{
    public class InternalDatabase
    {
        private static InternalDatabase instance;
        private static List<Employee> onlineEmployees;
        private static List<Team> teams;

        private InternalDatabase()
        {
            onlineEmployees = new List<Employee>();

            using (var access = new AccessDB())
            {
                teams = access.Teams.ToList();
            }
        }

        public static InternalDatabase Instance
        {
            get
            {
                if (instance == null)
                    instance = new InternalDatabase();

                return instance;
            }
            set
            {
                if (instance == null)
                    instance = value;
            }
        }

        public List<Employee> OnlineEmployees
        {
            get { return onlineEmployees; }
            set { onlineEmployees = value; }
        }

        public List<Team> Teams
        {
            get { return teams; }
            set { teams = value; }
        }
    }
}
