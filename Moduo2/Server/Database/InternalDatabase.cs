using ClientCommon.Data;
using System.Collections.Generic;
using System.Linq;

namespace Server.Database
{
    public class InternalDatabase
    {
        private static InternalDatabase instance;
        private static List<Employee> onlineEmployees;
        private object lockerOnlineEmployees;
        
        private InternalDatabase()
        {
            onlineEmployees = new List<Employee>();
            lockerOnlineEmployees = new object();
        }

        public static InternalDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InternalDatabase();
                }
                return instance;
            }
        }

        public List<Employee> OnlineEmployees
        {
            get { return onlineEmployees; }
            set { onlineEmployees = value; }
        }
        
        public object LockerOnlineEmployees
        {
            get { return this.lockerOnlineEmployees; }
            set { lockerOnlineEmployees = value; }
        }

        //Methods
        public void UpdateOnlineEmployee(Employee employee)
        {
            Employee onlineEmployee = OnlineEmployees.FirstOrDefault(e => e.Email.Equals(employee.Email));

            if (onlineEmployee != null)
            {
                onlineEmployee.Name = employee.Name;
                onlineEmployee.Surname = employee.Surname;
                onlineEmployee.WorkingHoursStart = employee.WorkingHoursStart;
                onlineEmployee.WorkingHoursEnd = employee.WorkingHoursEnd;
                onlineEmployee.Email = employee.Email;
            }
        }

        public void UpdateDeveloperToTL(Employee developer, Team newTeam)
        {
            Employee onlineEmployee = OnlineEmployees.FirstOrDefault(e => e.Email.Equals(developer.Email));

            if (onlineEmployee != null)
            {
                onlineEmployee.Type = EmployeeType.TEAMLEADER;
                onlineEmployee.Team = newTeam;
            }
        }
    }
}