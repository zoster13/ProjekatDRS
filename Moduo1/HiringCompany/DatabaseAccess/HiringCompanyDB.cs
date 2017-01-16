using EmployeeCommon;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICommon;
using EmployeeCommon.Data;

namespace HiringCompany.DatabaseAccess
{
    public class HiringCompanyDB : IHiringCompanyDB
    {
        private static IHiringCompanyDB myDB;

        // add lock before every adding, replacing, updating query
        // and add lock for every in-memory list, map..


        // lock objects for synchronizing access to ServiceDB.mdf
        private object allEmployees_lock = new object();
        private object projects_lock = new object();
        private object partnerCompanies_lock = new object();
       
        private HiringCompanyDB()
        {
        }

        public static IHiringCompanyDB Instance
        {
            get
            {
                if (myDB == null)
                {
                    myDB = new HiringCompanyDB();
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

        public object AllEmployees_lock
        {
            get { return allEmployees_lock; }
            set { allEmployees_lock = value; }
        }

        public object Projects_lock
        {
            get { return projects_lock; }
            set { projects_lock = value; }
        }

        public object PartnerCompanies_lock
        {
            get { return partnerCompanies_lock; }
            set { partnerCompanies_lock = value; }
        }

        public List<Employee> AllEmployees()
        {
            using (var access = new AccessDB())
            {
                List<Employee> retVal = new List<Employee>();

                lock (allEmployees_lock)
                {
                    var employees = from em in access.Employees

                                    select em;
                    retVal = employees.ToList();
                }

                return retVal;              
            }
            
        }

        public List<PartnerCompany> PartnerCompanies()
        {

            using (var access = new AccessDB())
            {
                List<PartnerCompany> retVal = new List<PartnerCompany>();
                lock (partnerCompanies_lock)
                {
                    retVal = access.Companies.ToList();
                }
                return retVal;               
            }
            
        }

        public List<Project> ProjectsForCeoApproval()
        {

            using (var access = new AccessDB())
            {
                List<Project> retVal = new List<Project>();
                lock (projects_lock)
                {
                    // get all Projects that are not yet approved by CEO
                    var projectsForA = from proj in access.Projects
                                       where proj.IsAcceptedCEO == false
                                       select proj;
                    retVal = projectsForA.ToList();
                }

                return retVal;
            }         
        }

        public List<Project> ProjectsForSendingToOutsC()
        {
            // get all Projects that are approved by CEO, and not assigned to any Outsorcing Company
            using (var access = new AccessDB())
            {
                List<Project> retVal = new List<Project>();
                lock (projects_lock)
                {
                    var projectsForS = from proj in access.Projects
                                       where proj.IsAcceptedCEO == true && proj.IsAcceptedOutsCompany == false
                                       select proj;

                    retVal = projectsForS.ToList();
                }
                return retVal;
            }           
        }

        public bool AddNewEmployee(Employee employee)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: HiringCompanyDB.AddNewEmployee" +
                                              "Employee:  string username={0}, string password={0}", employee.Username, employee.Password);
            Program.Logger.Info(messageToLog);

            bool retVal = false;
            try
            {
                using (var access = new AccessDB())
                {
                    int i = 0;
                    if (!access.Employees.Any(e => e.Username == employee.Username)) 
                    {
                        access.Employees.Add(employee); // does not exist in db
                        i = access.SaveChanges();
                        messageToLog = "added new employee in .mdf database.";
                                        Program.Logger.Info(messageToLog);
                    }
                    if (i > 0)
                    {
                        retVal = true;
                    }                       
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Program.Logger.Info(messageToLog);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                        Program.Logger.Info(messageToLog);
                    }
                }
            }
            return retVal;
        }

        public Employee GetEmployee(string username)
        {
            using (var access = new AccessDB())
            {
                var employee = from em in access.Employees.Include("Notifications") // dodala include 
                               where em.Username.Equals(username)
                               select em;

                return employee.ToList().FirstOrDefault();
            }
        }

        public bool AddNewPartnerCompany(PartnerCompany company)
        {
            string messageToLog = string.Empty;
            bool retVal = false;
            try
            {
                using (var access = new AccessDB())
                {
                    int i = 0;
                    if (!access.Companies.Any(c => c.Name == company.Name))
                    {
                        lock (partnerCompanies_lock)
                        {
                            access.Companies.Add(company);
                            i = access.SaveChanges();
                        }

                    }

                    if (i > 0)
                    {
                        retVal = true;
                    }                       
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Program.Logger.Info(messageToLog);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                        Program.Logger.Info(messageToLog);
                    }
                }
            }
            return retVal;
        }

        public bool AddNewProject(Project project)
        {
            bool retVal = false;
            try
            {
                using (var access = new AccessDB())
                {
                    int i = 0;
                    if (!access.Projects.Any(p => p.Id == project.Id))
                    {
                        lock (projects_lock)
                        {
                            access.Projects.Add(project);
                            i = access.SaveChanges();
                        }

                    }

                    if (i > 0)
                    {
                        retVal = true;
                    }                      
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
            }
            return retVal;
        }

        public bool EditEmployeeData(string username, string name, string surname, string email, string password)
        {
            bool retVal = false;
            string messageToLog = string.Empty;
            try
            {
                using (var access = new AccessDB())
                {
                    int i = 0;
                    Employee em = access.Employees.SingleOrDefault(e => e.Username.Equals(username));

                    if (em != null)
                    {
                        em.Name = name != "" ? name : em.Name;
                        em.Surname = surname != "" ? surname : em.Surname;
                        em.Email = email != "" ? email : em.Email;
                        em.Password = password != "" ? password : em.Password;
                        if (em.Password == password)
                        {
                            em.DatePasswordChanged = DateTime.Now;
                        }
                        i=access.SaveChanges();
                        messageToLog = "updated employee data in .mdf database.";
                        Program.Logger.Info(messageToLog);
                    }
                    if (i > 0)
                    {
                        retVal = true;
                    }
                }               
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Program.Logger.Info(messageToLog);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                        Program.Logger.Info(messageToLog);
                    }
                }
            }

            return retVal;
        }

        public bool EditWorkingHours(string username, int beginH, int beginM, int endH, int endM)
        {
            string messageToLog = string.Empty;
            bool retVal = false;
            try
            {
                using (var access = new AccessDB())
                {
                    int i = 0;
                    Employee em = access.Employees.SingleOrDefault(e => e.Username.Equals(username));
                    if (em != null)
                    {
                        em.StartHour = beginH;
                        em.StartMinute = beginM;
                        em.EndHour = endH;
                        em.EndMinute = endM;
                        i = access.SaveChanges();
                        messageToLog = "updated working hours data in .mdf database.";
                    }
                    if (i > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
            }

            return retVal;
        }

        public bool EditEmployeeType(string username, EmployeeType type)
        {
            string messageToLog = string.Empty;
            bool retVal = false;
            try
            {
                using (var access = new AccessDB())
                {
                    int i = 0;
                    Employee em = access.Employees.SingleOrDefault(e => e.Username.Equals(username));
                    if (em != null)
                    {
                        em.Type = type;
                        access.SaveChanges();
                        messageToLog = "employee type changed in .mdf database.";
                        Program.Logger.Info(messageToLog);
                    }
                    if (i > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Program.Logger.Info(messageToLog);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                        Program.Logger.Info(messageToLog);
                    }
                }
            }

            return retVal;
        }

        public bool ProjectApprovedCEOFieldChange(Project p)
        {
            string messageToLog = string.Empty;
            bool retVal = false;
            try
            {
                using (var access = new AccessDB())
                {
                    int i = 0;
                    var project = from proj in access.Projects
                                  where proj.Name.Equals(p.Name)
                                  select proj;

                    var pr = project.ToList().FirstOrDefault();
                    pr.IsAcceptedCEO = true;
                    i=access.SaveChanges();

                    messageToLog = "project propertu IsAcceptedCEO updated in .mdf database.";
                    Program.Logger.Info(messageToLog);
                    if (i > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Program.Logger.Info(messageToLog);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                        Program.Logger.Info(messageToLog);
                    }
                }
            }
            return retVal;
        }

        public bool CloseProjectFieldChange(string projectName)
        {
            string messageToLog = string.Empty;
            
            bool retVal = false;

            Project proj = new Project();
            using (var access = new AccessDB())
            {
                int i = 0;
                proj = access.Projects.SingleOrDefault(project => project.Name.Equals(projectName));
                proj.IsClosed = true;
                i=access.SaveChanges();
                if (i > 0)
                {
                    retVal = true;
                }

                messageToLog = "Updated Project.IsClosed data in .mdf database.";
                Program.Logger.Info(messageToLog);
            }
            return retVal;
        }

        public bool ResponseForProjectRequestFieldsChange(string outsourcingCompanyName, ProjectCommon p)
        {
            string messageToLog = string.Empty;
            string notification = string.Empty;
            bool retVal = false;
            Project proj = new Project();
            try
            {
                using (var access = new AccessDB())
                {
                    int i = 0;
                    proj = access.Projects.SingleOrDefault(project => project.Name.Equals(p.Name));
                    if (proj != null)
                    {
                        if (p.IsAcceptedByOutsCompany)
                        {
                            proj.OutsourcingCompany = outsourcingCompanyName;
                            proj.IsAcceptedOutsCompany = true;
                            i = access.SaveChanges();
                            messageToLog = "updated Project.IsAcceptedOutsCompanu data in .mdf database.";

                            Program.Logger.Info(messageToLog);
                        }
                    }
                    if (i > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Program.Logger.Info(messageToLog);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                        Program.Logger.Info(messageToLog);
                    }
                }
            }

            notification = p.IsAcceptedByOutsCompany ?
                string.Format("Company <" + outsourcingCompanyName + "> accepted request for developing project <" + p.Name + ">.") :
                string.Format("Company <" + outsourcingCompanyName + "> declined request for developing project <" + p.Name + ">.");


            using (Notifier notifier = new Notifier())
            {
                if (p.IsAcceptedByOutsCompany)
                {
                    notifier.SyncAll();
                }
                notifier.NotifySpecialClient(proj.ProductOwner, notification);
                notifier.NotifySpecialClients(EmployeeType.CEO, notification);
            }

            return retVal;
        }

        public bool SendUserStoriesToHiringCompanyFieldsChange(List<UserStory> tempUserStories, string projectName)
        {
            string messageToLog = string.Empty;
            string notification = string.Empty;
            bool retVal = false;
            Project proj = new Project();
            using (var access = new AccessDB())
            {
                int i = 0;      
                proj = access.Projects.SingleOrDefault(project => project.Name.Equals(projectName));
                if (proj != null)
                {
                    proj.UserStories = tempUserStories;
                    i=access.SaveChanges();

                    messageToLog = "Updated Project.UserStories data in .mdf database.";
                    Program.Logger.Info(messageToLog);
                    notification = string.Format("{0} User stories for project <{1}>, are waiting to be approved.", tempUserStories.Count, projectName);
                }
                if (i > 0)
                {
                    retVal = true;
                }
            }

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
                notifier.NotifySpecialClient(proj.ProductOwner, notification);
            }
            return retVal;
        }

        public bool SendClosedUserStoryFieldChange(string projectName, string title)
        {
            string notification = string.Empty;
            string notification2 = string.Empty;
            string messageToLog = string.Empty;
            bool retVal = false;

            Project proj = new Project();
            using (var access = new AccessDB())
            {
                int i = 0;
                proj = access.Projects.Include("UserStories").SingleOrDefault(p => p.Name.Equals(projectName));
                if (proj != null)
                {
                    UserStory us = proj.UserStories.Find(u => u.Title.Equals(title));
                    us.IsClosed = true;
                    i=access.SaveChanges();
                    notification = string.Format("User story <{0}> for project <{1}> is closed.", title, projectName);
                    messageToLog = "Updated Project.UserStories data in .mdf database.";
                    Program.Logger.Info(messageToLog);
                }

                if (i > 0)
                {
                    retVal = true;
                }
            }

            using (var access = new AccessDB())
            {
                proj = access.Projects.Include("UserStories").SingleOrDefault(p => p.Name.Equals(projectName));
                List<UserStory> notClosedUserStories = proj.UserStories.FindAll(u => u.IsClosed == false); //ovo ne radi kod mene na kompu, sacuva mi u bazi da je u userStory
                if (notClosedUserStories.Count == 0 && proj.UserStories.Count != 0)
                {
                    notification2 = string.Format("Project <{0}> can be closed, because all its user stories are closed.", projectName);
                }
            }

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
                notifier.NotifySpecialClient(proj.ProductOwner, notification);
                if (!notification2.Equals(String.Empty))
                {
                    notifier.NotifySpecialClient(proj.ProductOwner, notification2);
                }
            }
            return retVal;
        }

        public bool ClearEmployeeNotifs(string username)
        {
            string messageToLog = string.Empty;
            using (var access = new AccessDB())
            {
                bool retVal = false;
                int i = 0;
                var e = from ems in access.Employees.Include("Notifications")
                        where ems.Username.Equals(username)
                        select ems;
                if (e != null)
                {
                    var em = e.ToList().FirstOrDefault();
                    em.Notifications.Clear();
                    i=access.SaveChanges();

                    messageToLog = "Notifications data cleared in .mdf.";
                    Program.Logger.Info(messageToLog);
                }

                if (i > 0)
                {
                    retVal = true;
                }
                return retVal;
            }
        }
        public List<Project> ProjectsInDevelopment()
        {
            // get all Projects that are approved by CEO, and not assigned to any Outsorcing Company
            using (var access = new AccessDB())
            {
                var projects = access.Projects.Include("UserStories");

                var projectsInDev = from proj in projects
                                    where proj.IsAcceptedCEO == true && proj.IsAcceptedOutsCompany == true
                                    select proj;

                return projectsInDev.ToList();
            }           
        }

        public List<string> GetPartnerCompaniesNames()
        {
            using (var access = new AccessDB())
            {

                var pCompaniesName = from comp in access.Companies
                                     select comp.Name;

                return pCompaniesName.ToList();
            }
        }

        public Project SendApprovedUserStoriesFieldChange(string projectName, List<UserStory> userStories)
        {
            string messageToLog = string.Empty;
            Project proj = new Project();
            
            try
            {
                using (var access = new AccessDB())
                {
                    proj = access.Projects.Include("UserStories").SingleOrDefault(p => p.Name.Equals(projectName));

                    if (proj != null)
                    {
                        if (proj.UserStories.Count == userStories.Count)
                        {
                            for (int i = 0; i < proj.UserStories.Count; i++)
                            {
                                proj.UserStories[i].IsApprovedByPO = userStories[i].IsApprovedByPO;
                            }
                        }
                        else
                        {
                            messageToLog = "Unsuccessful idea! :( ";
                            Program.Logger.Info(messageToLog);
                        }

                        access.SaveChanges();

                        messageToLog = "changed user stories data in .mdf database.";
                        Program.Logger.Info(messageToLog);
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Program.Logger.Info(messageToLog);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                        Program.Logger.Info(messageToLog);
                    }
                }
            }
            return proj;
        }

        public bool RemoveDeclinedUserStoriesFromDB(string projectName)
        {
            string messageToLog = string.Empty;
            Project proj = new Project();
            bool retVal = true;
            try
            {
                // ovo treba da radimo u bazi 
                using (var access = new AccessDB())
                {
                    int i = 0;
                    proj = access.Projects.Include("UserStories").SingleOrDefault(p => p.Name.Equals(projectName));
                    proj.UserStories.RemoveAll(us => us.IsApprovedByPO == false);
                    i = access.SaveChanges();

                    messageToLog = "Removed user stories that were not approved in .mdf database.";
                    Program.Logger.Info(messageToLog);

                    if (i > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Program.Logger.Info(messageToLog);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                        Program.Logger.Info(messageToLog);
                    }
                }
            }
            return retVal;
        }
    }
}
