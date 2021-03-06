﻿using ClientCommon;
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
using ClientCommon.TempStructure;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class EmployeeService : IEmployeeService
    {
        #region Fields
        public static readonly log4net.ILog Logger = LogHelper.GetLogger();

        private Timer lateOnJobTimer = new Timer();
        private Timer passwordExpiredTimer = new Timer();
        private Timer userstoryCompleted = new Timer();

        private string _senderEmailAddress = "blok4.moduo2@gmail.com";
        private string _senderPassword = "ftnnovisad";

        private NetTcpBinding binding;
        private string hiringCompanyAddress;
        private List<Employee> allEmployees;
        private Team teamInDB;
        private Employee employeeInDB;

        private readonly string outsourcingCompanyName = "cekic";
        #endregion Fields

        //Constructor
        public EmployeeService()
        {
            allEmployees = new List<Employee>();
            teamInDB = new Team();
            employeeInDB = new Employee();
            hiringCompanyAddress = "net.tcp://10.1.212.113:9998/HiringService";
            binding = new NetTcpBinding();
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            lateOnJobTimer.Elapsed += new ElapsedEventHandler(NotifyOnLate);
            lateOnJobTimer.Interval = 15000;
            //lateOnJobTimer.Enabled = true;        

            passwordExpiredTimer.Elapsed += new ElapsedEventHandler(PasswordExpired);
            passwordExpiredTimer.Interval = 30000;
            //passwordExpiredTimer.Enabled = true;

            userstoryCompleted.Elapsed += new ElapsedEventHandler(UserStoryCompleted);
            userstoryCompleted.Interval = 30000;
            //userstoryCompleted.Enabled = true;
        }

        #region IEmployeeService Methods

        /// <summary>
        /// Logovanje zaposlenih na servis
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public void LogIn(string email, string password)
        {
            Logger.Info(string.Format("User [{0}] is trying to log in.", email));

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
                    Logger.Warn(string.Format("Employee [{0}] isn't loged in. Incorrect password.", email));
                    Publisher.Instance.LogInCallback(employee, false);
                }
            }
            else
            {
                Logger.Warn(string.Format("Employee [{0}] isn't loged in. There is no Employee in database with this credentials.", email));
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
                    teamInDB = EmployeeServiceDatabase.Instance.UpdateTeamScrumMaster(employee.Team.Name, employee.Email);

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
                Logger.Warn(string.Format("Employee [{0}] cannot be added to database.", employee.Name));
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
                Logger.Error("Team cannot be added to database.");
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
                    Logger.Error(string.Format("Team [{0}] isn't added to database.", team.Name));
                    Publisher.Instance.AddTeamAndTLCallback(null, null);
                }
            }
            else
            {
                Logger.Error(string.Format("Team cannot be added to database."));
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
                Logger.Error(string.Format("Team cannot be added to database."));
                Publisher.Instance.AddTeamAndTLCallback(null, null);
            }
        }

        /// <summary>
        /// Vracanje liste svih ulogovanih zaposlenih
        /// </summary>
        /// <returns></returns>
        public List<Employee> GetAllOnlineEmployees()
        {
            Logger.Info("Mehod GetAllOnlineEmployees() is called.");
            return InternalDatabase.Instance.OnlineEmployees;
        }

        /// <summary>
        /// Vracanje liste svih korisnickih prica
        /// </summary>
        /// <returns></returns>
        public List<UserStory> GetUserStories()
        {
            Logger.Info("Mehod GetUserStories() is called.");
            return EmployeeServiceDatabase.Instance.GetUserStories();
        }

        /// <summary>
        /// Vracanje liste svih zadataka
        /// </summary>
        /// <returns></returns>
        public List<Task> GetAllTasks()
        {
            Logger.Info("Mehod GetAllTasks() is called.");
            return EmployeeServiceDatabase.Instance.GetAllTasks();
        }

        /// <summary>
        /// Vracanje liste svih zaposlenih
        /// </summary>
        /// <returns></returns>
        public List<Employee> GetAllEmployees()
        {
            Logger.Info("Mehod GetAllEmployees() is called.");
            return EmployeeServiceDatabase.Instance.GetAllEmployees();
        }

        /// <summary>
        /// Vracanje liste svih timova
        /// </summary>
        /// <returns></returns>
        public List<Team> GetAllTeams()
        {
            Logger.Info("Mehod GetAllTeams() is called.");
            return EmployeeServiceDatabase.Instance.GetAllTeams();
        }

        /// <summary>
        /// Vracanje liste svih partnerskih kompanija 
        /// </summary>
        /// <returns></returns>
        public List<HiringCompany> GetAllHiringCompanies()
        {
            Logger.Info("Mehod GetAllHiringCompanies() is called.");
            return EmployeeServiceDatabase.Instance.GetAllHiringCompanies();
        }

        /// <summary>
        /// Vracanje svih projekata
        /// </summary>
        /// <returns></returns>
        public List<Project> GetAllProjects()
        {
            Logger.Info("Mehod GetAllProjects() is called.");
            return EmployeeServiceDatabase.Instance.GetAllProjects();
        }

        /// <summary>
        /// Dodjeljivanje projekta timu
        /// </summary>
        /// <param name="project"></param>
        public void ProjectTeamAssign(Project project)
        {
            project.Team = EmployeeServiceDatabase.Instance.UpdateProjectsTeam(project.Name, project.Team.Name);
            project.AssignStatus = AssignStatus.ASSIGNED;

            Logger.Info(string.Format("Project [{0}] is assigned to Team [{1}]", project.Name, project.Team.Name));

            //Obavjesti TL ako je online
            Employee teamLeader = InternalDatabase.Instance.OnlineEmployees.FirstOrDefault(e => e.Email.Equals(project.Team.TeamLeaderEmail));

            if (teamLeader != null)
            {
                Logger.Info(string.Format("Team leader is informed"));
                project.Team.Projects = new List<Project>();
                Publisher.Instance.ProjectTeamAssignCallback(project);
            }
        }

        /// <summary>
        /// Dodavanje nove korisnicke price u bazu
        /// </summary>
        /// <param name="userStory"></param>
        /// <param name="projectName"></param>
        public void AddUserStory(UserStory userStory, string projectName)
        {
            bool added = EmployeeServiceDatabase.Instance.AddUserStory(userStory, projectName);

            if (added)
            {
                Logger.Info(string.Format("UserStory [{0}] is added to database.", userStory.Title));
            }
            else
            {
                Logger.Error(string.Format("UserStory [{0}] isn't added to database.", userStory.Title));
            }
        }

        /// <summary>
        /// Dodavanje novog taska u bazu
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(Task task)
        {
            bool added = EmployeeServiceDatabase.Instance.AddTask(task);

            if (added)
            {
                Logger.Info(string.Format("Task [{0}] is added to database.", task.Title));
            }
            else
            {
                Logger.Error("Task cannot be added to database.");
            }
        }

        /// <summary>
        /// Otpustanje zavrene korisnicke price
        /// </summary>
        /// <param name="userStory"></param>
        public void ReleaseUserStory(UserStory userStory)
        {
            userStory = EmployeeServiceDatabase.Instance.ReleaseUserStory(userStory);
            Logger.Info(string.Format("User story [{0}] is released.", userStory.Title));

            Publisher.Instance.ReleaseUserStoryCallback(userStory.Tasks);
        }

        /// <summary>
        /// Prihvatanje taskova
        /// </summary>
        /// <param name="task"></param>
        public void TaskClaimed(Task task)
        {
            task = EmployeeServiceDatabase.Instance.TaskClaimed(task);

            Logger.Info(string.Format("Task [{0}] is claimed.", task.Title));
            Publisher.Instance.TaskClaimedCallback(task);
        }

        /// <summary>
        /// Zavrsavanje taskova
        /// </summary>
        /// <param name="task"></param>
        public void TaskCompleted(Task task)
        {
            TaskAndUserStoryCompletedFlag retvalue = EmployeeServiceDatabase.Instance.TaskCompleted(task);

            Logger.Info(string.Format("Task [{0}] is completed.", retvalue.Task.Title));
            Publisher.Instance.TaskCompletedCallback(retvalue.Task);

            if (retvalue.UserStoryCompletedFlag)
            {
                using (var proxy = new ServerProxy.ServerProxy(binding, hiringCompanyAddress))
                {
                    proxy.SendClosedUserStory(retvalue.Task.UserStory.Project.Name, retvalue.Task.UserStory.Title);
                }
            }
        }

        /// <summary>
        /// Slanje korisnickikh prica
        /// </summary>
        /// <param name="userStories"></param>
        /// <param name="projectName"></param>
        public void SendUserStories(List<UserStoryCommon> userStories, string projectName)
        {
            bool updated = EmployeeServiceDatabase.Instance.UpdateProjectStatus(projectName);

            if (updated)
            {
                Logger.Info("Project status is updated to STARTED.");

                using (var proxy = new ServerProxy.ServerProxy(binding, hiringCompanyAddress))
                {
                    proxy.SendUserStoriesToHiringCompany(userStories, projectName);
                }
            }
            else
            {
                Logger.Warn("Project status isn't updated to STARTED.");
            }
        }

        #endregion IEmployeeService Methods


        #region Responses to Hiring company

        public void ResponseToPartnershipRequest(bool accepted, string hiringCompanyName)
        {
            if (accepted)
            {
                HiringCompany newHiringCompany = new HiringCompany(hiringCompanyName);

                EmployeeServiceDatabase.Instance.AddHiringCompany(newHiringCompany);
                Publisher.Instance.ResponseToPartnershipRequestCallback(newHiringCompany);

                Logger.Info("New partnership company is added.");
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
                project.Team = null;
                EmployeeServiceDatabase.Instance.AddProject(project);

                Logger.Info("New project is added to system.");
            }

            ProjectCommon prCommon = new ProjectCommon();
            prCommon.Name = project.Name;
            prCommon.IsAcceptedByOutsCompany = accepted;

            using (var proxy = new ServerProxy.ServerProxy(binding, hiringCompanyAddress))
            {
                proxy.ResponseForProjectRequest(outsourcingCompanyName, prCommon);
            }
        }

        #endregion Responses to Hiring company

        #region Timers

        /// <summary>
        /// Provjera da li zaposleni kasni na posao
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NotifyOnLate(object sender, ElapsedEventArgs e)
        {
            allEmployees = EmployeeServiceDatabase.Instance.GetAllEmployees();

            foreach (Employee em in allEmployees)
            {
                if (!InternalDatabase.Instance.OnlineEmployees.Contains(em))
                {
                    TimeSpan timeDiff = DateTime.Now - em.WorkingHoursStart;
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
        /// Provjera da li password istekao
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PasswordExpired(object sender, ElapsedEventArgs e)
        {
            allEmployees = GetAllEmployees();

            foreach (Employee em in allEmployees)
            {
                if (em.PasswordTimeStamp.AddMinutes(3) < DateTime.Now)
                {
                    var client = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(_senderEmailAddress, _senderPassword),
                        EnableSsl = true
                    };
                    client.Send(_senderEmailAddress, em.Email, "Obavjestenje", "Vasa lozinka je istekla! Morate je promjeniti!");
                    Logger.Info(string.Format("Employee [{0}] must change password.", em.Name));
                }
            }
        }

        /// <summary>
        /// Provjera da li je US zavrsen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UserStoryCompleted(object sender, ElapsedEventArgs e)
        {
            List<UserStory> allUserStories = EmployeeServiceDatabase.Instance.GetAllUserStories();
            int numOfClosedTasks = 0;

            foreach (var us in allUserStories)
            {
                if (DateTime.Now.AddDays(2) > us.Deadline)
                {
                    foreach (var task in us.Tasks)
                    {
                        if (task.ProgressStatus == ProgressStatus.COMPLETED)
                        {
                            numOfClosedTasks++;
                        }
                    }

                    if (numOfClosedTasks < ((us.Tasks.Count * 80) / 100))
                    {
                        Logger.Info(string.Format("Isn't finished 80% taks for user story [{0}] 2 days before deadline.", us.Title));
                        NotifySMForUserStoryProgress(us.Project.Team.ScrumMasterEmail, us.Title);
                    }
                }
            }
        }

        public void NotifySMForUserStoryProgress(string scrumMasterEmail, string userStoryName)
        {
            //Posalji mu notifikaciju ako je online
            Employee scrumMaster = InternalDatabase.Instance.OnlineEmployees.FirstOrDefault(e => e.Email.Equals(scrumMasterEmail));

            if (scrumMaster != null)
            {
                Publisher.Instance.NotifySMForUserStoryProgressCallback(scrumMasterEmail, userStoryName);
            }
            else
            {
                //Posalji mail
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(_senderEmailAddress, _senderPassword),
                    EnableSsl = true
                };
                client.Send(_senderEmailAddress, scrumMasterEmail, "Obavjestenje", string.Format("Nije zavrseno 80% zadataka za UserStory [{0}].", userStoryName));
                Logger.Info(string.Format("Nije zavrseno 80% zadataka za UserStory [{0}].", userStoryName));
            }
        }

        #endregion Timers
    }
}