using ClientCommon.Data;
using System.Linq;
using System.Data.SqlClient;
using System;

namespace Server.Access
{
    public class EmployeeServiceDatabase : IEmployeeServiceDatabase
    {
        private static IEmployeeServiceDatabase myDB;
        private static object lockObjectEmployees;
        private static object lockObjectTeams;
        private static object lockObjectNotifications;

        public static IEmployeeServiceDatabase Instance
        {
            get
            {
                if (myDB == null)
                {
                    myDB = new EmployeeServiceDatabase();
                    lockObjectEmployees = new object();
                    lockObjectTeams = new object();
                    lockObjectNotifications = new object();
                }

                return myDB;
            }

            set
            {
                if (myDB == null)
                {
                    myDB = value;
                }
            }
        }

        public bool AddEmployee(Employee employee)
        {
            using (var access = new AccessDB())
            {
                Employee employeeInDB = access.Employees.FirstOrDefault(e => e.Email.Equals(employee.Email));

                //dodaj samo ako ne postoji u bazi
                if (employeeInDB == null)
                {
                    if (employee.Team != null)
                    {
                        Team team = access.Teams.FirstOrDefault(t => t.Name.Equals(employee.Team.Name));
                        employee.Team = team;
                    }

                    lock (lockObjectEmployees)
                    {
                        access.Employees.Add(employee);
                        access.SaveChanges();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void UpdateEmployeeFunctionAndTeam(Employee employee, string newTeamName)
        {
            using (var access = new AccessDB())
            {
                Team newTeam = access.Teams.FirstOrDefault(t => t.Name.Equals(newTeamName));

                Employee employeeInDB = access.Employees
                    .Include("Team")
                    .FirstOrDefault(e => e.Email.Equals(employee.Email));

                lock (lockObjectEmployees)
                {
                    employeeInDB.Team = newTeam;
                    employeeInDB.Type = EmployeeType.TEAMLEADER;

                    access.SaveChanges();
                }
            }
        }

        public Employee GetEmployee(string email)
        {
            Employee employeeInDB = null;

            using (var access = new AccessDB())
            {
                employeeInDB = access.Employees
                    .Include("Team")
                    .Include("Notifications")
                    .FirstOrDefault(e => e.Email.Equals(email));

                employeeInDB.Team = access.Teams
                    .Include("Projects")
                    .FirstOrDefault(t => t.Name.Equals(employeeInDB.Team.Name));


                foreach (Project proj in employeeInDB.Team.Projects)
                {
                    proj.UserStories = access.UserStories
                        .Include("Tasks")
                        .Where(us => us.Project == proj).ToList();
                }
            }

            return employeeInDB;
        }

        public bool AddTeam(Team team)
        {
            using (var access = new AccessDB())
            {
                var team1 = from t in access.Teams
                            where t.Name.Equals(team.Name)
                            select t;

                if (team1.ToList().FirstOrDefault() == null)
                {
                    lock (lockObjectTeams)
                    {
                        access.Teams.Add(team);
                        access.SaveChanges();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Team GetTeam(string name)
        {
            using (var access = new AccessDB())
            {
                var team = from t in access.Teams
                               where t.Name.Equals(name)
                               select t;

                return team.ToList().First();
            }
        }

        public void UpdateEmployee(Employee employee)
        {
            using (var access = new AccessDB())
            {
                Employee employeeInDB = access.Employees
                    .Include("Team")
                    .FirstOrDefault(e => e.Email.Equals(employee.Email));

                lock (lockObjectEmployees)
                {
                    employeeInDB.Email = employee.Email;
                    employeeInDB.Name = employee.Name;
                    employeeInDB.Surname = employee.Surname;
                    employeeInDB.WorkingHoursStart = employee.WorkingHoursStart;
                    employeeInDB.WorkingHoursEnd = employee.WorkingHoursEnd;
                    employeeInDB.Password = employee.Password;

                    access.SaveChanges();
                }
            }
        }
        
        public void AddNotification(Notification notification)
        {
            using (var access = new AccessDB())
            {
                Employee ceo = access.Employees.FirstOrDefault(e => e.Id == notification.Emoloyee.Id);
                notification.Emoloyee = ceo;

                lock (lockObjectNotifications)
                {
                    access.Notifications.Add(notification);
                }
                access.SaveChanges();
            }
        }
    }
}
