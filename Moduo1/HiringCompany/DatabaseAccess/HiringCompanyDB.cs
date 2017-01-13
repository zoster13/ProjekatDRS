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
    public class HiringCompanyDB
    {
        private static HiringCompanyDB myDB;

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
        private object partnerCompanies_lock=new object();

        // lock objects for synchronizing access to data about active communication channels
        private object channelsCompanies_lock=new object();
        private object channelsClients_lock=new object();

        //  lock objects for synchronizing access to in-memory data
        private object onlineEmployees_lock = new object();
        private object projectsForApproval_lock = new object(); 
        private object partnerCompaniesAddresses_lock = new object();
        private object dbAccess_lock = new object(); // verovatno nam ne treba, zato sto svaku tabelu stitimo zasebno tamo gde se koristi


        // in-memory data
        // videti gde cuvati ove podatke, mozda na pocetku setup delu iscitati iz fajla sve, neki fajl ili slicno.. 
        //mzftn123fakultet@gmail.com
        //miljanazvezdana123
        private string companyName = "HiringCompany";  
        private List<Employee> onlineEmployees;
        
        // [Key="companyName", Value="ipaddress:port" ]
        private Dictionary<string, string> possiblePartnersAddresses; 
        private Dictionary<string, string> partnerCompaniesAddresses; 

        private Dictionary<string, OutsorcingCompProxy> connectionChannelsCompanies; 
        private Dictionary<string, IEmployeeServiceCallback> connectionChannelsClients; 


        private HiringCompanyDB()
        {

            onlineEmployees = new List<Employee>();

            possiblePartnersAddresses = new Dictionary<string, string>();
            partnerCompaniesAddresses = new Dictionary<string, string>();

            connectionChannelsClients = new Dictionary<string, IEmployeeServiceCallback>();
            connectionChannelsCompanies = new Dictionary<string, OutsorcingCompProxy>();
     

            // u fajlu cuvati, i onda iscitati na pocetku programa
            possiblePartnersAddresses.Add("cekic", "10.1.212.114:9998"); 
            possiblePartnersAddresses.Add("bluc", "10.1.212.114:9998"); // outs company ce nam reci podatke

            List<string> pCompaniesName = new List<string>();
            using (var access = new AccessDB())
            {
                pCompaniesName = (from comp in access.Companies
                                  select comp.Name).ToList();
            }

            foreach (string cName in pCompaniesName)
            {
                partnerCompaniesAddresses.Add(cName, possiblePartnersAddresses[cName]);
                possiblePartnersAddresses.Remove(cName);
            }
        }

        public static HiringCompanyDB Instance()
        {
            if (myDB == null)
            {
                myDB = new HiringCompanyDB();
            }
            return myDB;
        }

        public object AllEmployees_lock
        {
            get { return allEmployees_lock; }
        }
        public object Projects_lock
        {
            get { return projects_lock;}
        }
        public object PartnerCompanies_lock
        {
            get { return partnerCompanies_lock;}
        }

        public object ChannelsCompanies_lock
        {
            get { return channelsCompanies_lock; }
        }

        public object ChannelsClients_lock
        {
            get { return channelsClients_lock;}
        }

        public object OnlineEmployees_lock
        {
            get { return onlineEmployees; }
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
       
        // da li ovde streba da stitim u geteru? msm mozda neka druga nit bas tad dodaje nekog employee u online
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
        }

        public Dictionary<string, string> PossiblePartnersAddresses
        {
            get { return possiblePartnersAddresses; }
            set { possiblePartnersAddresses = value; }
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
                    List<PartnerCompany> retVal=new List<PartnerCompany>();
                    lock (partnerCompanies_lock)
                    {
                        retVal = access.Companies.ToList();
                    }
                    return retVal;
                   // return access.Companies.ToList();
                }
            }
        }

        public List<Project> ProjectsForCeoApproval
        {
            get
            {
                using (var access = new AccessDB())
                {
                    List<Project> retVal=new List<Project>();
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
        }
        public List<Project> ProjectsForSendingToOutsC
        {
            get
            {
                // get all Projects that are approved by CEO, and not assigned to any Outsorcing Company
                using (var access = new AccessDB())
                {
                    List<Project> retVal=new List<Project>();
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
        }

        // treba lock ovde?
        public Dictionary<string, IEmployeeServiceCallback> ConnectionChannelsClients
        {
            get
            {
                return connectionChannelsClients;
            }
            set { connectionChannelsClients = value; }
        }
        public Dictionary<string, OutsorcingCompProxy> ConnectionChannelsCompanies
        {
            get { return connectionChannelsCompanies; }
            set { connectionChannelsCompanies = value; }
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

        public List<Project> ProjectsInDevelopment
        {
            get
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
}
