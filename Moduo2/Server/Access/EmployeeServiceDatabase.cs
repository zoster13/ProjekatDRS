using ClientCommon.Data;
using System.Linq;
using System.Collections.Generic;
using ClientCommon.TempStructure;
using System;
using ICommon;

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

        public bool UpdateEmployeeFunctionAndTeam(Employee employee, string newTeamName)
        {
            using (var access = new AccessDB())
            {
                Team newTeam = access.Teams.FirstOrDefault(t => t.Name.Equals(newTeamName));

                if (newTeam != null)
                {
                    Employee employeeInDB = access.Employees
                        .Include("Team")
                        .FirstOrDefault(e => e.Email.Equals(employee.Email));

                    lock (lockObjectEmployees)
                    {
                        employeeInDB.Team = newTeam;
                        employeeInDB.Type = EmployeeType.TEAMLEADER;

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

        public Employee GetEmployee(string email)
        {
            Employee employeeInDB = null;
            
            using (var access = new AccessDB())
            {
                employeeInDB = access.Employees
                    .Include("Team")
                    .Include("Notifications")
                    .FirstOrDefault(e => e.Email.Equals(email));

                //if (employeeInDB.Type == EmployeeType.CEO)
                //{
                //    return employeeInDB;
                //}
                //else
                //{
                //    employeeInDB.Team = access.Teams
                //        .Include("Projects")
                //        .FirstOrDefault(t => t.Name.Equals(employeeInDB.Team.Name));

                //   foreach (Project proj in employeeInDB.Team.Projects)
                //    {
                //        proj.UserStories = access.UserStories
                //            .Include("Tasks")
                //            .Where(us => us.Project.Name.Equals(proj.Name)).ToList();
                //    }
                //}
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

        public bool UpdateEmployee(Employee employee)
        {
            using (var access = new AccessDB())
            {
                Employee employeeInDB = access.Employees
                    .Include("Team")
                    .FirstOrDefault(e => e.Email.Equals(employee.Email));

                if (employeeInDB != null)
                {
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
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        public void AddNotification(Notification notification)
        {
            using (var access = new AccessDB())
            {
                List<Employee> allEmployees = access.Employees.Include("Team").ToList();

                foreach (Employee emp in allEmployees)
                {
                    if (emp.Type.Equals(EmployeeType.CEO))
                    {
                        emp.Team = null;
                        notification.Emoloyee = emp;

                        lock (lockObjectNotifications)
                        {
                            access.Notifications.Add(notification);
                        }
                        access.SaveChanges();
                    }
                }
            }
        }

        //izmjestanje
        public Team UpdateTeamScrumMaster(string teamName, string scrumMasterEmail)
        {
            using (var access = new AccessDB())
            {
                Team teamInDB = access.Teams.FirstOrDefault(t => t.Name.Equals(teamName));

                teamInDB.ScrumMasterEmail = scrumMasterEmail;
                access.SaveChanges();

                return teamInDB;
            }
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
            using (var access = new AccessDB())
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

        public List<Project> GetAllProjects()
        {
            using (var access = new AccessDB())
            {
                return access.Projects.Include("Team").ToList();
            }
        }

        public Team UpdateProjectsTeam(string projectName, string teamName)
        {
            using (var access = new AccessDB())
            {
                Project proj = access.Projects.FirstOrDefault(p => p.Name.Equals(projectName));
                Team team = access.Teams.FirstOrDefault(t => t.Name.Equals(teamName));

                proj.Team = team;
                proj.AssignStatus = AssignStatus.ASSIGNED;
                
                access.SaveChanges();

                return team;
            }
        }

        public bool AddUserStory(UserStory userStory, string projectName)
        {
            using (var access = new AccessDB())
            {
                Project proj = access.Projects.Include("Team").FirstOrDefault(p => p.Name.Equals(projectName));
            
                userStory.Project = proj;
                access.UserStories.Add(userStory);
                int i = access.SaveChanges();

                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool AddTask(Task task)
        {
            using (var access = new AccessDB())
            {
                UserStory userStory = access.UserStories.Include("Project").FirstOrDefault(us => us.Title.Equals(task.UserStory.Title));
                userStory.Project = access.Projects.Include("Team").FirstOrDefault(p => p.Name.Equals(userStory.Project.Name));
                task.UserStory = userStory;

                access.Tasks.Add(task);
                int i = access.SaveChanges();

                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public UserStory ReleaseUserStory(UserStory userStory)
        {
            using (var access = new AccessDB())
            {
                UserStory userStoryInDB = access.UserStories.Include("Tasks").Include("Project").FirstOrDefault(us => us.Title.Equals(userStory.Title));
                userStoryInDB.Project = access.Projects.Include("Team").FirstOrDefault(p => p.Name.Equals(userStoryInDB.Project.Name));

                userStoryInDB.ProgressStatus = ProgressStatus.STARTED;
                userStoryInDB.Deadline = userStory.Deadline;

                foreach (var task in userStoryInDB.Tasks)
                {
                    Task taskInDB = access.Tasks.FirstOrDefault(t => t.Title.Equals(task.Title));
                    taskInDB.ProgressStatus = ProgressStatus.STARTED;
                }
                //userStory.Tasks = userStoryInDB.Tasks;
                access.SaveChanges();

                return userStoryInDB;
            }
        }

        public Task TaskClaimed(Task task)
        {
            // Prponaci task prema title i postaviti da je claimed i started, i postaviti ime employee-a
            // callback TaskClaimedCallback(Task) za sve clanove tima, vratiti taj task

            using (var access = new AccessDB())
            {
                Task taskInDB = access.Tasks.Include("UserStory").FirstOrDefault(t => t.Title.Equals(task.Title));
                taskInDB.UserStory = access.UserStories.Include("Project").FirstOrDefault(us => us.Title.Equals(taskInDB.UserStory.Title));
                taskInDB.UserStory.Project = access.Projects.Include("Team").FirstOrDefault(p => p.Name.Equals(taskInDB.UserStory.Project.Name));

                taskInDB.AssignStatus = AssignStatus.ASSIGNED;
                taskInDB.ProgressStatus = ProgressStatus.STARTED;
                taskInDB.EmployeeName = task.EmployeeName;

                access.SaveChanges();

                return taskInDB;
            }
        }

        public TaskAndUserStoryCompletedFlag TaskCompleted(Task task)
        {
            TaskAndUserStoryCompletedFlag returnValue = new TaskAndUserStoryCompletedFlag();
            returnValue.UserStoryCompletedFlag = true;

            using (var access = new AccessDB())
            {
                Task taskInDB = access.Tasks.Include("UserStory").FirstOrDefault(t => t.Title.Equals(task.Title));
                taskInDB.UserStory = access.UserStories.Include("Project").FirstOrDefault(us => us.Title.Equals(taskInDB.UserStory.Title));
                taskInDB.UserStory.Project = access.Projects.Include("Team").FirstOrDefault(p => p.Name.Equals(taskInDB.UserStory.Project.Name));

                taskInDB.ProgressStatus = ProgressStatus.COMPLETED;

                //access.SaveChanges();

                taskInDB.UserStory = access.UserStories.Include("Tasks").Include("Project").FirstOrDefault(us => us.Title.Equals(taskInDB.UserStory.Title));

                foreach (var task1 in taskInDB.UserStory.Tasks)
                {
                    if (task1.ProgressStatus != ProgressStatus.COMPLETED)
                    {
                        returnValue.UserStoryCompletedFlag = false;
                        break;
                    }
                }

                taskInDB.UserStory.ProgressStatus = ProgressStatus.COMPLETED;

                access.SaveChanges();

                returnValue.Task = taskInDB;

                return returnValue;
            }
        }

        public bool UpdateProjectStatus(string projectName)
        {
            using (var access = new AccessDB())
            {
                Project projectInDB = access.Projects.Include("Team").FirstOrDefault(p => p.Name.Equals(projectName));
                projectInDB.ProgressStatus = ProgressStatus.PENDING;

                int i = access.SaveChanges();

                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public void AddHiringCompany(HiringCompany hiringCompany)
        {
            using (var access = new AccessDB())
            {
                access.HiringCompanies.Add(hiringCompany);
                access.SaveChanges();
            }
        }

        public void AddProject(Project project)
        {
            using (var access = new AccessDB())
            {
                access.Projects.Add(project);
                access.SaveChanges();
            }
        }

        public List<UserStory> GetUserStories()
        {
            using (var access = new AccessDB())
            {
                return access.UserStories.Include("Project").ToList();
            }
        }

        public List<Task> GetAllTasks()
        {
            using (var access = new AccessDB())
            {
                return access.Tasks.Include("UserStory").ToList();
            }
        }

        public Project UpdateUserStoriesStatus(List<UserStoryCommon> commUserStories, string projectName)
        {
            Project proj;

            using (var access = new AccessDB())
            {
                proj = access.Projects
                    .Include("UserStories")
                    .Include("Team")
                    .FirstOrDefault(p => p.Name.Equals(projectName));

                proj.ProgressStatus = ProgressStatus.STARTED;

                foreach (var userStory in commUserStories)
                {
                    UserStory usInDB = proj.UserStories.FirstOrDefault(us => us.Title.Equals(userStory.Title));

                    if (usInDB != null)
                    {
                        if (userStory.IsAccepted)
                        {
                            usInDB.AcceptStatus = AcceptStatus.ACCEPTED;
                        }
                        else
                        {
                            usInDB.AcceptStatus = AcceptStatus.DECLINED;
                        }
                    }
                }
                access.SaveChanges();

                return proj;
            }
        }

        public List<UserStory> GetAllUserStories()
        {
            using (var access = new AccessDB())
            {
                List<UserStory> userStories = access.UserStories
                    .Include("Tasks")
                    .Include("Project")
                    .ToList();

                foreach (var us in userStories)
                {
                    us.Project = access.Projects.Include("Team").FirstOrDefault(p => p.Name.Equals(us.Project.Name));
                }

                return userStories;
            }
        }
    }
}
