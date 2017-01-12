using EmployeeCommon;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICommon;

namespace HiringCompany.DatabaseAccess
{
    public class HiringCompanyDB
    {
        private static HiringCompanyDB myDB;
        // add lock before every adding, replacing, updating query
        // and add lock for every in-memory list, map..

        // da ne budu public objekti
        private object onlineEmployees_lock = new object();
        private object allEmployees_lock = new object(); 
        private object projectsForApproval_lock = new object(); 
        private object partnerCompaniesAddresses_lock = new object();
        private object dbAccess_lock = new object(); // LOKOVATI SVUDA GDE SE KORISTI DB ACCESS!!!!!!!

        // in-memory data
        // videti gde cuvati ove podatke, mozda na pocetku setup delu iscitati iz fajla sve, neki fajl ili slicno.. 
        //mzftn123fakultet@gmail.com
        //miljanazvezdana123
        private string companyName = "HiringCompany";
        private List<Employee> onlineEmployees;
        private Dictionary<string, string> partnerCompaniesAddresses; // ["companyName", "ipaddress:port"]
        private Dictionary<string, OutsorcingCompProxy> connectionChannelsCompanies; // treba zakljucavati
        private Dictionary<string, IEmployeeServiceCallback> connectionChannelsClients; // treba zakljucavati


        private HiringCompanyDB()
        {
            onlineEmployees = new List<Employee>();
            partnerCompaniesAddresses = new Dictionary<string, string>();
            connectionChannelsClients = new Dictionary<string, IEmployeeServiceCallback>();
            connectionChannelsCompanies = new Dictionary<string, OutsorcingCompProxy>();

            partnerCompaniesAddresses.Add("cekic", "10.1.212.114:9998"); // u fajlu cuvati
        }

        public object OnlineEmployees_lock
        {
            get { return onlineEmployees; }
        }

        public object AllEmployees_lock
        {
            get { return allEmployees_lock; }
        }

        public object ProjectsForApproval_lock
        {
            get { return projectsForApproval_lock; }
        }

        public object PartnerCompaniesAddresses_lock
        {
            get { return partnerCompaniesAddresses_lock; }
        }

        public object DbAccess_lock
        {
            get { return dbAccess_lock; }
        }

        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }
        public List<Employee> OnlineEmployees
        {
            get { return onlineEmployees; }
            set { onlineEmployees = value; }
        }

        public List<Employee> AllEmployees
        {
            get
            {
                using (var access = new AccessDB())
                {
                    // zakljucati ovde
                    var employees = from em in access.Employees

                                    select em;

                    return employees.ToList();
                }
            }
        }

        public Dictionary<string, string> PartnerCompaniesAddresses
        {
            get { return partnerCompaniesAddresses; }
            set { partnerCompaniesAddresses = value; }
        }

        public List<PartnerCompany> PartnerCompanies
        {
            get
            {
                using (var access = new AccessDB())
                {

                    return access.Companies.ToList();

                }
            }
        }

        public List<Project> ProjectsForCeoApproval
        {
            get
            {
                using (var access = new AccessDB())
                {
                    // get all Projects that are not yet approved by CEO
                    var projectsForA = from proj in access.Projects
                                       where proj.IsAcceptedCEO == false
                                       select proj;

                    //if (projectsForA.ToList() == null) // videti gde jos treba ova provera
                    //{
                    //    return new List<Project>();
                    //}
                    //else
                    //{
                    //    return projectsForA.ToList();
                    //}
                    return projectsForA.ToList();
                }
            }
        }
        public List<Project> ProjectsForSendingToOutsC
        {
            get
            {
                // get all Projects that are approved by CEO, and not assigned to any Outsorcing Company
                using (var access = new AccessDB())
                {
                    //var projects = access.projects.Include("UserStories"); //mozda mora ovako da se radi sa include
                  
                    //var projectsForS = from proj in projects

                    var projectsForS = from proj in access.Projects
                                       where proj.IsAcceptedCEO == true && proj.IsAcceptedOutsCompany == false
                                       select proj;

                    //if (projectsForS.ToList() == null) // videti gde jos treba ova provera
                    //{
                    //    return new List<Project>();
                    //}
                    //else
                    //{
                    //    return projectsForS.ToList();
                    //}
                    return projectsForS.ToList();
                }
            }
        }

        public Dictionary<string, IEmployeeServiceCallback> ConnectionChannelsClients
        {
            get { return connectionChannelsClients; }
            set { connectionChannelsClients = value; }
        }
        public Dictionary<string, OutsorcingCompProxy> ConnectionChannelsCompanies
        {
            get { return connectionChannelsCompanies; }
            set { connectionChannelsCompanies = value; }
        }

        public static HiringCompanyDB Instance()
        {
            if (myDB == null)
            {
                myDB = new HiringCompanyDB();
            }               
            return myDB;
        }

        // valjda treba da ima neka metoda za brisanje employee-a? 

        public bool AddNewEmployee(EmployeeCommon.Employee employee)
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
                        access.Companies.Add(company);
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
                        access.Projects.Add(project);
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

        public List<Project> ProjectsInDevelopment
        {
            get
            {
                // get all Projects that are approved by CEO, and not assigned to any Outsorcing Company
                using (var access = new AccessDB())
                {
                    var projects = access.Projects.Include("UserStories"); //mozda mora ovako da se radi sa include

                    //var projectsInDev = from proj in access.Projects
                    var projectsInDev = from proj in projects
                                        where proj.IsAcceptedCEO == true && proj.IsAcceptedOutsCompany == true
                                        select proj;

                    return projectsInDev.ToList();
                }
            }
        }
    }
}
