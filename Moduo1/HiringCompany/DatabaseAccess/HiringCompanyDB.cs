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
        private InternalDatabase internalDatabase = InternalDatabase.Instance();

        // add lock before every adding, replacing, updating query
        // and add lock for every in-memory list, map..

        // problem i sa ovim lockovanjem. ne treba tako da se radi...kada sam guglala,
        // izbacilo je hiljadu drugih nacina -> nigde nije radjeno sa lock-om. 
        // https://stackoverflow.com/questions/9415955/c-sharp-working-with-entity-framework-in-a-multi-threaded-server
        // cak pisu i da ne treba sa tim da se radi. jos nesto, zeka i aleksandra su lockovali samo
        // na pisanju, ali treba i na citanju. To mi bas nije imalo smisla sto su radili pa sam proverila i treba da se stiti i citanje
        // ako stitimo citanje, teze je da dodjem do podataka koje treba da vratim :S, vidi dole AllEmployes


        // lock objects for synchronizing access to ServiceDB.mdf
        private object allEmployees_lock = new object();
        private object projects_lock = new object();
        private object partnerCompanies_lock = new object();

        private object dbAccess_lock = new object(); // verovatno nam ne treba, zato sto svaku tabelu stitimo zasebno tamo gde se koristi

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

        public object DbAccess_lock
        {
            get { return dbAccess_lock; }
            set { dbAccess_lock = value; }
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
                // return employees.ToList();
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
                // return access.Companies.ToList();
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
                // return projectsForA.ToList();
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
                //return projectsForS.ToList();
            }           
        }

        public bool AddNewEmployee(Employee employee)
        {
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

        public Employee GetEmployee(string username)
        {
            using (var access = new AccessDB())
            {
                var employee = from em in access.Employees
                               where em.Username.Equals(username)
                               select em;

                return employee.ToList().FirstOrDefault();
            }
        }

        public bool AddNewPartnerCompany(PartnerCompany company)
        {
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

            lock (internalDatabase.OnlineEmployees_lock)
            {
                Employee em = internalDatabase.OnlineEmployees.Find(e => e.Username.Equals(username));
                if (em != null)
                {
                    em.Name = name != "" ? name : em.Name;
                    em.Surname = surname != "" ? surname : em.Surname;
                    em.Email = email != "" ? email : em.Email;
                    em.Password = password != "" ? password : em.Password;
                    messageToLog = "updated employee data in OnlineEmployees list.";
                    Program.Logger.Info(messageToLog);
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

            lock (internalDatabase.OnlineEmployees_lock)
            {
                Employee em = internalDatabase.OnlineEmployees.Find(e => e.Username.Equals(username));
                if (em != null)
                {
                    em.StartHour = beginH;
                    em.StartMinute = beginM;
                    em.EndHour = endH;
                    em.EndMinute = endM;
                    messageToLog = "updated working hours data in OnlineEmployees list.";
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

            lock (internalDatabase.OnlineEmployees_lock)
            {
                Employee em = internalDatabase.OnlineEmployees.Find(e => e.Username.Equals(username));
                if (em != null)
                {
                    em.Type = type;
                    messageToLog = "employee type changed in OnlineEmployees list";
                    Program.Logger.Info(messageToLog);
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
    }
}
