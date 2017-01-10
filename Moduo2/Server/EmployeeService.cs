using ClientCommon;
using ClientCommon.Data;
using Server.Access;
using Server.Database;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Data.SqlClient;
using System.Timers;
using System.Net.Mail;
using System.Net;
using System.ServiceModel;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class EmployeeService : IEmployeeService
    {
        Timer lateOnJobTimer = new Timer();

        public EmployeeService()
        {
            lateOnJobTimer.Elapsed += new ElapsedEventHandler(NotifyOnLate);
            lateOnJobTimer.Interval = 15000;
            //lateOnJobTimer.Enabled = true;        
        }

        private void NotifyOnLate(object sender, ElapsedEventArgs e)
        {
            string _senderEmailAddress = "blok4.moduo2@gmail.com";
            string _senderPassword = "ftnnovisad";
            Console.WriteLine("alarm...");
            foreach (Employee em in InternalDatabase.Instance.AllEmployees) // slanje maila onima koji nisu online
            {
                if (!InternalDatabase.Instance.OnlineEmployees.Contains(em))
                {
                    DateTime current = DateTime.Now;
                    DateTime workTimeEmployee = em.WorkingHoursStart;
                    TimeSpan timeDiff = current - workTimeEmployee;
                    TimeSpan allowed = new TimeSpan(0, 15, 0);

                    if (timeDiff > allowed)
                    {
                        var client = new SmtpClient("smtp.gmail.com", 587)
                        {
                            Credentials = new NetworkCredential(_senderEmailAddress, _senderPassword),
                            EnableSsl = true
                        };
                        client.Send(_senderEmailAddress, em.Email, "Obavjestenje", "Zakasnili ste na posao!");
                    }
                }
            }
        }

        public void LogIn(string email, string password)
        {
            Employee employee = EmployeeServiceDatabase.Instance.GetEmployee(email);

            if(employee!=null && password.Equals(employee.Password))
            {
                lock (InternalDatabase.Instance.LockerOnlineEmployees)
                {
                    InternalDatabase.Instance.OnlineEmployees.Add(employee);
                }

                Publisher.Instance.LogInCallback(employee);
            }
        }

        public void LogOut(Employee employee)
        {
            foreach (Employee e in InternalDatabase.Instance.OnlineEmployees)
            {
                if (e.Email.Equals(employee.Email))
                {
                    lock (InternalDatabase.Instance.LockerOnlineEmployees)
                    {
                        InternalDatabase.Instance.OnlineEmployees.Remove(e);
                    }
                    break;
                }
            }

            Employee employeeInDB = EmployeeServiceDatabase.Instance.GetEmployee(employee.Email);

            using (var access = new AccessDB())
            {
                foreach (Notification notif in access.Notifications.ToList())
                {
                    if (notif.Emoloyee == employeeInDB)
                    {

                    }
                }
            }

            Publisher.Instance.LogOutCallback(employee);
        }
        
        public List<Employee> GetAllOnlineEmployees()
        {
            return InternalDatabase.Instance.OnlineEmployees;
        }

        public List<Employee> GetAllEmployees()
        {
            using (var access = new AccessDB())
            {
                return access.Employees.ToList();
            }
        }

        public List<Team> GetAllTeams()
        {
            return InternalDatabase.Instance.Teams;
        }

        public List<HiringCompany> GetAllHiringCompanies()
        {
            throw new NotImplementedException();
        }

        public void AddTeam(Team team)
        {
            if (EmployeeServiceDatabase.Instance.AddTeam(team))
            {
                lock (InternalDatabase.Instance.LockerTeams)
                {
                    InternalDatabase.Instance.Teams.Add(team);
                }

                Publisher.Instance.TeamAddedCallback(team, true);
            }
            else
            {
                Publisher.Instance.TeamAddedCallback(team, false);
            }
        }

        public void EditEmployeeData(Employee employee)
        {
            EmployeeServiceDatabase.Instance.UpdateEmployee(employee);
            //dodati i update u internoj bazi!!!

            Publisher.Instance.EditEmployeeCallback(employee);
        }

        public void ProjectTeamAssign(string projName, string teamName)
        {
            throw new NotImplementedException();
        }

        public void AddEmployee(Employee employee)
        {
            EmployeeServiceDatabase.Instance.AddEmployee(employee);
            Publisher.Instance.AddEmployeeCallback(employee);

            if (employee.Type.Equals(EmployeeType.SCRUMMASTER))
            {
                employee.Team.ScrumMasterEmail = employee.Email;
                EmployeeServiceDatabase.Instance.UpdateScrumMaster(employee);

                Publisher.Instance.ScrumMasterUpdatedCallback(employee.Team);
            }
        }

        public void UpdateEmployeeFunctionAndTeam(Employee employee, string newTeamName)
        {
            EmployeeServiceDatabase.Instance.UpdateEmployeeFunctionAndTeam(employee, newTeamName);

            Publisher.Instance.UpdateEmployeeFunctionAndTeamCallback(employee);
            Publisher.Instance.NotifyJustMe(employee);
        }
    }
}