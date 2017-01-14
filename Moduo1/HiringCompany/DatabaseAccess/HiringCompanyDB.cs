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
