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
        private readonly string outsourcingCompanyName = "cekic";
        private Timer lateOnJobTimer = new Timer();
        private NetTcpBinding binding;
        private string hiringCompanyAddress;
        List<Employee> allEmployees;
        private Team teamInDB;
        private Employee employeeInDB;
        #endregion Fields

        //Constructor
        public EmployeeService()
        {
            allEmployees = new List<Employee>();
            teamInDB = new Team();
            employeeInDB = new Employee();
            hiringCompanyAddress = "net.tcp://10.1.212.13:9998/HiringService";
            binding = new NetTcpBinding();
            //binding.OpenTimeout = new TimeSpan(1, 0, 0);
            //binding.CloseTimeout = new TimeSpan(1, 0, 0);
            //binding.SendTimeout = new TimeSpan(1, 0, 0);
            //binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            lateOnJobTimer.Elapsed += new ElapsedEventHandler(NotifyOnLate);
            lateOnJobTimer.Interval = 15000;
            //lateOnJobTimer.Enabled = true;        
        }


        #region IEmployeeService Methods

        /// <summary>
        /// Logovanje zaposlenih na servis
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public void LogIn(string email, string password)
        {
            Employee employee = EmployeeServiceDatabase.Instance.GetEmployee(email);

            if (employee != null)
            {
                if (password.Equals(employee.Password))
                {
                    if (!InternalDatabase.Instance.OnlineEmployees.Contains(employee))
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
                        Logger.Info(string.Format("Employee [{0}] is already loged in.", email));
                        Publisher.Instance.LogInCallback(null);
                    }
                }
                else
                {
                    Logger.Info(string.Format("Employee [{0}] isn't loged in. Incorrect password.", email));
                    Publisher.Instance.LogInCallback(null);
                }
            }
            else
            {
                Logger.Info(string.Format("Employee [{0}] isn't loged in. There is no Employee in database with this credentials.", email));
                Publisher.Instance.LogInCallback(null);
            }
        }

        /// <summary>
        /// Odjavljivanje zaposlenih sa servisa
        /// </summary>
        /// <param name="employee"></param>
        public void LogOut(Employee employee)
        {
            Employee loggedInEmployee = InternalDatabase.Instance.OnlineEmployees.FirstOrDefault(e => e.Email.Equals(employee.Email));
            
            lock (InternalDatabase.Instance.LockerOnlineEmployees)
            {
                InternalDatabase.Instance.OnlineEmployees.Remove(loggedInEmployee);
            }
            
            //DODATI AZURIRANJE NOTIFIKACIJA
            /*
            using (var access = new AccessDB())
            {
                employeeInDB = EmployeeServiceDatabase.Instance.GetEmployee(employee.Email);
                List<Notification> notificationsInDB = access.Notifications.Where(n => n.Emoloyee == employeeInDB).ToList();
                
                foreach(Notification notif in employee.Notifications)
                {
                    
                }
            }
            */

            Logger.Info(string.Format("Employee [{0}] is loged out.", employee.Email));
            Publisher.Instance.LogOutCallback(employee);
        }
        
        /// <summary>
        /// Dodavanje novog tima u bazu
        /// </summary>
        /// <param name="team"></param>
        public void AddTeam(Team team)
        {
            bool teamAdded = EmployeeServiceDatabase.Instance.AddTeam(team);

            if (teamAdded)
            {
                Logger.Info(string.Format("Team [{0}] is added to database.", team.Name));
                Publisher.Instance.TeamAddedCallback(team);
            }
            else
            {
                Logger.Info(string.Format("Team [{0}] isn't added to database.", team.Name));
                Publisher.Instance.TeamAddedCallback(null);
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
            using (var access = new AccessDB())
            {
                Project proj = access.Projects.FirstOrDefault(p => p.Name.Equals(project.Name));
                Team team = access.Teams.FirstOrDefault(t => t.Name.Equals(project.Team.Name));

                proj.Team = team;
                proj.AssignStatus = AssignStatus.ASSIGNED;

                access.SaveChanges();
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
            using(var access = new AccessDB())
            {
                return access.Teams.ToList();
            }
        }

        public List<HiringCompany> GetAllHiringCompanies()
        {
            using (var access = new AccessDB())
            {
                return access.HiringCompanies.ToList();
            }
        }

        public void AddEmployee(Employee employee)
        {
            bool employeeAdded = EmployeeServiceDatabase.Instance.AddEmployee(employee);

            if(employeeAdded)
            {
                Logger.Info(string.Format("Employee [{0}] is added to database.", employee.Name));
                Publisher.Instance.AddEmployeeCallback(employee);

                if (employee.Type.Equals(EmployeeType.SCRUMMASTER))
                {
                    using (var access = new AccessDB())
                    {
                        teamInDB = access.Teams.FirstOrDefault(t => t.Name.Equals(employee.Team.Name));
                        teamInDB.ScrumMasterEmail = employee.Email;
                        access.SaveChanges();
                    }

                    Publisher.Instance.ScrumMasterUpdatedCallback(teamInDB);
                }
            }
            else
            {
                Logger.Info(string.Format("Employee [{0}] cannot be added to database.", employee.Name));
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

            using (var access = new AccessDB())
            {
                allEmployees = access.Employees.ToList();
            }

            foreach (Employee em in allEmployees)
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

        public void AddUserStory(UserStory userStory, string projectName)
        {
            // dodaje user story u bazu, ne treba callback
            using (var access = new AccessDB())
            {
                Project proj = access.Projects.FirstOrDefault(p => p.Name.Equals(projectName));

                userStory.Project = proj;
                access.UserStories.Add(userStory);
                access.SaveChanges();
            }

            Logger.Info(string.Format("UserStory [{0}] is added to database.", userStory.Title));
        }

        public void AddTask(Task task)
        {
            // izvuce se user story iz baze na osnovu user storija iz taska( story ima title) i doda se task
            // ne treba callback

            using (var access = new AccessDB())
            {
                UserStory userStory = access.UserStories.FirstOrDefault(us => us.Title.Equals(task.UserStory.Title));
                task.UserStory = userStory;

                access.Tasks.Add(task);
                access.SaveChanges();
            }
        }

        public void ReleaseUserStory(UserStory userStory)
        {
            // user story je sada kreirana potpuno zajedno sa taskovima i treba da se posalje svim clanovima tima 
            // kako bi oni mogli da preuzimaju taskove
            using (var access = new AccessDB())
            {
                UserStory userStory1 = access.UserStories.FirstOrDefault(us => us.Title.Equals(userStory.Title));
                userStory1.ProgressStatus = ProgressStatus.STARTED;

                foreach (var task in userStory.Tasks)
                {
                    Task taskInDB = access.Tasks.FirstOrDefault(t => t.Title.Equals(task.Title));
                    taskInDB.ProgressStatus = ProgressStatus.STARTED;

                    access.Tasks.Add(task);
                    //access.SaveChanges(); // ne znam da li treba
                }

                access.UserStories.Add(userStory1);
                access.SaveChanges();
            }

            Publisher.Instance.ReleaseUserStoryCallback(userStory);
        }

        public void TaskClaimed(Task task)
        {
            // rponaci task prema title i postaviti da je claimed i started, i postaviti ime employee-a
            // callback TaskClaimedCallback(Task) za sve clanove tima, vratiti taj task

            using (var access = new AccessDB())
            {
                Task taskInDB = access.Tasks.FirstOrDefault(t => t.Title.Equals(task.Title));

                taskInDB.AssignStatus = AssignStatus.ASSIGNED;
                taskInDB.ProgressStatus = ProgressStatus.STARTED;
                taskInDB.EmployeeName = task.EmployeeName;

                access.SaveChanges();
            }

            Logger.Info(string.Format("Task [{0}] is claimed.", task.Title));
            Publisher.Instance.TaskClaimedCallback(task);
        }

        public void TaskCompleted(Task task)
        {
            // slicno kao claimed
            using (var access = new AccessDB())
            {
                Task taskInDB = access.Tasks.FirstOrDefault(t => t.Title.Equals(task.Title));

                taskInDB.ProgressStatus = ProgressStatus.COMPLETED;

                access.SaveChanges();
            }

            Logger.Info(string.Format("Task [{0}] is completed.", task.Title));
            Publisher.Instance.TaskCompletedCallback(task);
        }

        public void SendUserStories(List<UserStoryCommon> userStories, string projectName)
        {
            // salje listu user storija za projekat
            // u nasu bazu abdejtuje status za projekat status pending
            // ne treba callback

            using (var access = new AccessDB())
            {
                Project projectInDB = access.Projects.FirstOrDefault(p => p.Name.Equals(projectName));
                projectInDB.ProgressStatus = ProgressStatus.PENDING;
                access.SaveChanges();
            }

            //Poziv metode iz ICommona...
        }
        
        #endregion IEmployeeService Methods

        #region Responses to Hiring company

        public void ResponseToPartnershipRequest(bool accepted, string hiringCompanyName)
        {
            if (accepted)
            {
                HiringCompany newHiringCompany = new HiringCompany(hiringCompanyName);

                using (var access = new AccessDB())
                {
                    access.HiringCompanies.Add(newHiringCompany);
                    access.SaveChanges();
                }
            }

            using (var proxy = new ServerProxy.ServerProxy(binding, hiringCompanyAddress))
            {
                proxy.ResponseForPartnershipRequest(accepted, outsourcingCompanyName);
            }

            //DODATI CALLBACK DA SE VRATI SVIM ONLINE KORISNICIMA NOVA HITING KOMPANIJA
        }

        public void ResponseToProjectRequest(bool accepted, Project project)
        {
            if (accepted)
            {
                Project newProject = new Project();
                newProject = project;

                using (var access = new AccessDB())
                {
                    access.Projects.Add(newProject);
                    access.SaveChanges();
                }
            }

            ProjectCommon prCommon = new ProjectCommon();
            prCommon.Name = project.Name;
            prCommon.IsAcceptedByOutsCompany = accepted;

            //using (var proxy = new ServerProxy.ServerProxy(binding, hiringCompanyAddress))
            //{
            //    proxy.ResponseForProjectRequest(outsourcingCompanyName, prCommon);
            //}
        }

        #endregion 
    }
}