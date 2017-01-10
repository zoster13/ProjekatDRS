using ClientCommon;
using ClientCommon.Data;
using Server.Access;
using Server.Database;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Timers;
using System.Net.Mail;
using System.Net;
using System.ServiceModel;
using Server.Logger;
using ICommon;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class EmployeeService : IEmployeeService
    {
        public static readonly log4net.ILog Logger = LogHelper.GetLogger();
        Timer lateOnJobTimer = new Timer();

        public EmployeeService()
        {
            lateOnJobTimer.Elapsed += new ElapsedEventHandler(NotifyOnLate);
            lateOnJobTimer.Interval = 15000;
            //lateOnJobTimer.Enabled = true;        
        }

        
        public void LogIn(string email, string password)
        {
            Employee employee = EmployeeServiceDatabase.Instance.GetEmployee(email);

            if(employee != null && password.Equals(employee.Password))
            {
                lock (InternalDatabase.Instance.LockerOnlineEmployees)
                {
                    InternalDatabase.Instance.OnlineEmployees.Add(employee);
                    Logger.Info(string.Format("Employee [{0}] is loged in.", email));
                }

                Publisher.Instance.LogInCallback(employee);
            }
            else
            {
                Logger.Info(string.Format("Employee [{0}] isn't loged in. There is no Employee in database with this credentials.", email));
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
                        Logger.Info(string.Format("Employee [{0}] is loged out.", employee.Email));
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
                        //DOVRSITI!!!
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

                Logger.Info(string.Format("Team [{0}] is added to database.", team.Name));
                Publisher.Instance.TeamAddedCallback(team, true);
            }
            else
            {
                Logger.Info(string.Format("Team [{0}] isn't added to database.", team.Name));
                Publisher.Instance.TeamAddedCallback(team, false);
            }
        }

        public void EditEmployeeData(Employee employee)
        {
            EmployeeServiceDatabase.Instance.UpdateEmployee(employee);
            //dodati i update u internoj bazi!!!

            Logger.Info(string.Format("Employee [{0}] is edited.", employee.Email));
            Publisher.Instance.EditEmployeeCallback(employee);
        }

        public void ProjectTeamAssign(Project project)
        {
            throw new NotImplementedException();

            // ako je tim lider online, treba mu poslati projekat (SAMO NJEMU), inace se stavlja u bazu
            // mora se postaviti referenca projekta tj. izvuci tim iz baze
        }

        public void AddEmployee(Employee employee)
        {
            EmployeeServiceDatabase.Instance.AddEmployee(employee);
            Logger.Info(string.Format("Employee [{0}] is added to database.", employee.Name));
            Publisher.Instance.AddEmployeeCallback(employee);

            if (employee.Type.Equals(EmployeeType.SCRUMMASTER))
            {
                employee.Team.ScrumMasterEmail = employee.Email;
                EmployeeServiceDatabase.Instance.UpdateScrumMaster(employee);
                Logger.Info(string.Format("ScrumMaster [{0}] is updated.", employee.Name));

                Publisher.Instance.ScrumMasterUpdatedCallback(employee.Team);
            }
        }

        public void UpdateEmployeeFunctionAndTeam(Employee employee, string newTeamName)
        {
            EmployeeServiceDatabase.Instance.UpdateEmployeeFunctionAndTeam(employee, newTeamName);
            Logger.Info(string.Format("Employe [{0}] is updated your function and team.", employee.Name));

            Publisher.Instance.UpdateEmployeeFunctionAndTeamCallback(employee);
            Publisher.Instance.NotifyJustMe(employee);
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
                        Logger.Info(string.Format("Employee [{0}] late on work.", em.Name));
                    }
                }
            }
        }

        public void AddUserStory(UserStory userStory)
        {
            // dodaje user story u bazu, ne treba callback
            throw new NotImplementedException();
        }



        public void AddTask(Task task)
        {
            // izvuce se user story iz baze na osnovu user storija iz taska( story ima title) i doda se task
            // ne treba callback
            throw new NotImplementedException();
        }


        public void ReleaseUserStory(UserStory userStory)
        {
            // user story je sada kreirana potpuno zajedno sa taskovima i treba da se posalje svim clanovima tima kako bi oni mogli da preuzimaju taskove
            throw new NotImplementedException();
        }

        public void TaskClaimed(Task task)
        {
            // rponaci task prema title i postaviti da je claimed i started, i postaviti ime employee-a
            // callback TaskClaimedCallback(Task) za sve clanove tima, vratiti taj task
            throw new NotImplementedException();
        }

        public void TaskCompleted(Task task)
        {
            // slicno kao claimed
            throw new NotImplementedException();
        }

        //IOutsourcingService
        public void ResponseToPartnershipRequest(bool accepted, string companyName)
        {
            Publisher.Instance.AskForPartnershipCallback(accepted, companyName);
        }

        public void SendUserStories(List<UserStoryCommon> userStories, string projectName)
        {
            // salje listu user storija za projekat
            // u nasu bazu abdejtuje status za projekat status pending
            // ne treba callback
            throw new NotImplementedException();
        }

        
    }
}