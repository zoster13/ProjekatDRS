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
        private static List<Employee> allEmployees;
        private static List<Team> teams;

        private object lockerOnlineEmployees;
        private object lockerAllEmployees;
        private object lockerTeams;


        private InternalDatabase()
        {
            onlineEmployees = new List<Employee>();
            allEmployees = new List<Employee>();

            lockerAllEmployees = new object();
            lockerOnlineEmployees = new object();
            lockerTeams = new object();

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

        public List<Employee> AllEmployees
        {
            get
            {
                using (var access = new AccessDB())
                {
                    var employees = from em in access.Employees

                                    select em;

                    return employees.ToList();
                }
            }
            set { allEmployees = value; }
        }

        public object LockerAllEmployees
        {
            get { return this.lockerAllEmployees; }
            set { lockerAllEmployees = value; }
        }
        public object LockerOnlineEmployees
        {
            get { return this.lockerOnlineEmployees; }
            set { lockerOnlineEmployees = value; }
        }
        public object LockerTeams
        {
            get { return this.lockerTeams; }
            set { lockerTeams = value; }
        }

    }
}
