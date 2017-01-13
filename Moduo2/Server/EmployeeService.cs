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

                            Publisher.Instance.LogInCallback(employee, true);
                        }
                        else
                        {
                            Logger.Info(string.Format("Employee [{0}] is already loged in.", email));
                            Publisher.Instance.LogInCallback(employee, false);
                        }
                    }
                    else
                    {
                        Logger.Info(string.Format("Employee [{0}] isn't loged in. Incorrect password.", email));
                        Publisher.Instance.LogInCallback(employee, false);
                    }
                }
                else
                {
                    Logger.Info(string.Format("Employee [{0}] isn't loged in. There is no Employee in database with this credentials.", email));
                    Publisher.Instance.LogInCallback(new Employee(), false);
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
        /// Dodavanje novog zaposlenog u sistem
        /// </summary>
        /// <param name="employee"></param>
        public void AddEmployee(Employee employee)
        {
            bool employeeAdded = EmployeeServiceDatabase.Instance.AddEmployee(employee);

            if (employeeAdded)
            {
                //Ako je SM, podesi timu ScrumMasterEmail
                if (employee.Type.Equals(EmployeeType.SCRUMMASTER))
                {
                    using (var access = new AccessDB())
                    {
                        teamInDB = access.Teams.FirstOrDefault(t => t.Name.Equals(employee.Team.Name));
                        teamInDB.ScrumMasterEmail = employee.Email;
                        access.SaveChanges();
                    }

                    Publisher.Instance.ScrumMasterAddedCallback(employee, teamInDB);
                }
                else
                {
                    Publisher.Instance.AddEmployeeCallback(employee);
                }

                Logger.Info(string.Format("Employee [{0}] is added to database.", employee.Name));
            }
            else
            {
                Logger.Info(string.Format("Employee [{0}] cannot be added to database.", employee.Name));
            }
        }

        /// <summary>
        /// Azuriranje izmjenjenih podataka zaposlenog u bazi
        /// </summary>
        /// <param name="employee"></param>
        public void EditEmployeeData(Employee employee)
        {
            EmployeeServiceDatabase.Instance.UpdateEmployee(employee);
            InternalDatabase.Instance.UpdateOnlineEmployee(employee);

            Logger.Info(string.Format("Employee [{0}] personal data are updated.", employee.Email));
            Publisher.Instance.EditEmployeeCallback(employee);
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

        /// <summary>
        /// Dodavanje novog tima i novog zaposlenog koji je tipa TL
        /// </summary>
        /// <param name="team"></param>
        /// <param name="teamLeader"></param>
        public void AddTeamAndTL(Team team, Employee teamLeader)
        {
            bool teamAdded = EmployeeServiceDatabase.Instance.AddTeam(team);

            if (teamAdded)
            {
                bool employeeAdded = EmployeeServiceDatabase.Instance.AddEmployee(teamLeader);

                if (employeeAdded)
                {
                    Logger.Info(string.Format("Team [{0}] is added to database.", team.Name));
                    Logger.Info(string.Format("Employee [{0}] is added to database.", teamLeader.Name));

                    Publisher.Instance.AddTeamAndTLCallback(team, teamLeader);
                }
                else
                {
                    Logger.Info(string.Format("Team [{0}] isn't added to database.", team.Name));
                    Publisher.Instance.AddTeamAndTLCallback(null, null);
                }
            }
            else
            {
                Logger.Info(string.Format("Team [{0}] isn't added to database.", team.Name));
                Publisher.Instance.AddTeamAndTLCallback(null, null);
            }
        }

        /// <summary>
        /// Dodavanje novog tima i postavljanje postojeceg Developera u TL
        /// </summary>
        /// <param name="team"></param>
        /// <param name="developer"></param>
        public void AddTeamAndUpdateDeveloperToTL(Team team, Employee developer)
        {
            bool teamAdded = EmployeeServiceDatabase.Instance.AddTeam(team);

            if (teamAdded)
            {
                EmployeeServiceDatabase.Instance.UpdateEmployeeFunctionAndTeam(developer, team.Name);

                InternalDatabase.Instance.UpdateDeveloperToTL(developer, team);

                Logger.Info(string.Format("Employee [{0}] is updated your function and team.", developer.Name));
                Logger.Info(string.Format("Team [{0}] is added to database.", team.Name));

                Publisher.Instance.AddTeamAndTLCallback(team, developer);
                Publisher.Instance.NotifyJustMe(developer);
            }
            else
            {
                Logger.Info(string.Format("Team [{0}] isn't added to database.", team.Name));
                Publisher.Instance.AddTeamAndTLCallback(null, null);
            }
        }

        /// <summary>
        /// Vracanje liste svih ulogovanih zaposlenih
        /// </summary>
        /// <returns></returns>
        public List<Employee> GetAllOnlineEmployees()
        {
            return InternalDatabase.Instance.OnlineEmployees;
        }

        /// <summary>
        /// Vracanje liste svih zaposlenih
        /// </summary>
        /// <returns></returns>
        public List<Employee> GetAllEmployees()
        {
            using (var access = new AccessDB())
            {
                return access.Employees.ToList();
            }
        }

        /// <summary>
        /// Vracanje liste svih timova
        /// </summary>
        /// <returns></returns>
        public List<Team> GetAllTeams()
        {
            using (var access = new AccessDB())
            {
                return access.Teams.ToList();
            }
        }

        /// <summary>
        /// Vracanje liste svih partnerskih kompanija 
        /// </summary>
        /// <returns></returns>
        public List<HiringCompany> GetAllHiringCompanies()
        {
            using (var access = new AccessDB())
            {
                return access.HiringCompanies.ToList();
            }
        }

        /// <summary>
        /// Dodjeljivanje projekta timu
        /// </summary>
        /// <param name="project"></param>
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
        
        /// <summary>
        /// Provjera da li zaposleni kasni na posao
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Dodavanje nove korisnicke price u bazu
        /// </summary>
        /// <param name="userStory"></param>
        /// <param name="projectName"></param>
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

        /// <summary>
        /// Dodavanje novog taska u bazu
        /// </summary>
        /// <param name="task"></param>
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

            using (var proxy = new ServerProxy.ServerProxy(binding, hiringCompanyAddress))
            {
                proxy.SendUserStoriesToHiringCompany(userStories, projectName);
            }
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

                Publisher.Instance.ResponseToPartnershipRequestCallback(newHiringCompany);
            }

            using (var proxy = new ServerProxy.ServerProxy(binding, hiringCompanyAddress))
            {
                proxy.ResponseForPartnershipRequest(accepted, outsourcingCompanyName);
            }
        }

        public void ResponseToProjectRequest(bool accepted, Project project)
        {
            if (accepted)
            {
                Project newProject = new Project();
                newProject = project;
                newProject.Team = null;

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