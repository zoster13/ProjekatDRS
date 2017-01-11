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
        #region Fields
        public static readonly log4net.ILog Logger = LogHelper.GetLogger();
        Timer lateOnJobTimer = new Timer();
        #endregion Fields

        public EmployeeService()
        {
            lateOnJobTimer.Elapsed += new ElapsedEventHandler(NotifyOnLate);
            lateOnJobTimer.Interval = 15000;
            //lateOnJobTimer.Enabled = true;        
        }

        #region IEmployeeService Methods
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

            using (var access = new AccessDB())
            {
                Employee employeeInDB = EmployeeServiceDatabase.Instance.GetEmployee(employee.Email);

                foreach (Notification notif in access.Notifications.ToList())
                {
                    if (notif.Emoloyee == employeeInDB)
                    {
                        //DOVRSITI!!! azurirati notifikacije
                    }
                }
            }

            Publisher.Instance.LogOutCallback(employee);
        }

        public void AddTeam(Team team)
        {
            bool teamAdded = EmployeeServiceDatabase.Instance.AddTeam(team);

            if (teamAdded)
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
            //Update u mdf bazi
            EmployeeServiceDatabase.Instance.UpdateEmployee(employee);
            
            //Update u internoj bazi
            Employee thisEmployee = InternalDatabase.Instance.OnlineEmployees.FirstOrDefault(e => e.Email.Equals(employee.Email));
            thisEmployee.Name = employee.Name;
            thisEmployee.Surname = employee.Surname;
            thisEmployee.WorkingHoursStart = employee.WorkingHoursStart;
            thisEmployee.WorkingHoursEnd = employee.WorkingHoursEnd;
            thisEmployee.Email = employee.Email;

            Logger.Info(string.Format("Employee [{0}] is edited.", employee.Email));
            Publisher.Instance.EditEmployeeCallback(employee);
        }

        public void ProjectTeamAssign(Project project)
        {
            // ako je tim lider online, treba mu poslati projekat (SAMO NJEMU), inace se stavlja u bazu
            // mora se postaviti referenca projekta tj. izvuci tim iz baze

            //Azuriraj tim projekta u bazi
            using(var access = new AccessDB())
            {
                Project proj = access.Projects.FirstOrDefault(p => p.Name.Equals(project.Name));
                Team team = access.Teams.FirstOrDefault(t => t.Name.Equals(project.Team.Name));

                proj.Team = new Team();
                proj.Team = team;
            }

            //Obavjesti TL ako je online
            Employee teamLeader = InternalDatabase.Instance.OnlineEmployees.FirstOrDefault(e => e.Email.Equals(project.Team.TeamLeaderEmail));

            if (teamLeader != null)
            {
                Publisher.Instance.ProjectTeamAssignCallback(project);
            }
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

        public void ResponseToPartnershipRequest(bool accepted, string companyName)
        {
            //ako prihvati dodaj u bazu
            if(accepted)
            {
                HiringCompany newHiringCompany = new HiringCompany(companyName);

                using(var access = new AccessDB())
                {
                    access.HiringCompanies.Add(newHiringCompany);
                }
            }

            Publisher.Instance.AskForPartnershipCallback(accepted, companyName);
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

        public void SendUserStories(List<UserStoryCommon> userStories, string projectName)
        {
            // salje listu user storija za projekat
            // u nasu bazu abdejtuje status za projekat status pending
            // ne treba callback
            throw new NotImplementedException();
        }
        #endregion IEmployeeService Methods
    }
}