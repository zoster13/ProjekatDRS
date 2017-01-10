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

        // zasto su public objecti?
        public object Employees_lock = new object();
        public object AllEmployees_lock = new object();
        public object ProjectsForApproval_lock = new object();
        public object PartnerCompaniesAddresses_lock = new object();
        public object DbAccess_lock = new object(); // LOKOVATI SVUDA GDE SE KORISTI DB ACCESS!!!!!!!

        // lockovi za connection channele
        // dodati lockove za pravu bazu

        private string companyName = "HiringCompany";// company name, videti gde ga cuvati, neki fajl ili slicno.. 
        private List<Employee> onlineEmployees;
        private List<Employee> allEmployees; // ne koristimo
        private List<Project> projectsForApproval; // u mdf
        private Dictionary<string, string> partnerCompaniesAddresses; // ["companyName", "ipaddress:port"]
        private Dictionary<string, OutsorcingCompProxy> connectionChannelsCompanies; // treba zakljucavati
        Dictionary<string, IEmployeeServiceCallback> connectionChannelsClients; // treba zakljucavati

        

        private HiringCompanyDB()
        {
            onlineEmployees = new List<Employee>();
            allEmployees = new List<Employee>();
            projectsForApproval = new List<Project>();
            partnerCompaniesAddresses = new Dictionary<string, string>();
            connectionChannelsClients = new Dictionary<string, IEmployeeServiceCallback>();
            connectionChannelsCompanies = new Dictionary<string, OutsorcingCompProxy>();

            partnerCompaniesAddresses.Add("ZekaMisa", "10.1.212.114:9998");
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
            get {
                using(var access = new AccessDB())
                {
                    // zakljucati ovde
                    var employees = from em in access.employees

                                    select em;

                    return employees.ToList();
                }
            }
            set { allEmployees = value; } // ne koristimo nigde
        }

        public List<PartnerCompany> PartnerCompanies
        {
            get
            {
                using (var access = new AccessDB())
                {
                    var companies = from com in access.companies
                                    select com;
                    return companies.ToList();
                }
            }
        }

        public List<Project> ProjectsForApproval
        {
            get { return projectsForApproval; }
            set { projectsForApproval = value; }
        }

        public Dictionary<string, IEmployeeServiceCallback> ConnectionChannelsClients
        {
            get { return connectionChannelsClients; }
            set { connectionChannelsClients = value; }
        }

        public Dictionary<string,string> PartnerCompaniesAddresses
        {
            get { return partnerCompaniesAddresses; }
            set { partnerCompaniesAddresses = value; }
        }

        public Dictionary<string, OutsorcingCompProxy> ConnectionChannelsCompanies
        {
            get { return connectionChannelsCompanies; }
            set { connectionChannelsCompanies = value; }
        }

        public static HiringCompanyDB Instance()
        {
            if(myDB == null)
                myDB = new HiringCompanyDB();
            return myDB;
        }

        

        // valjda treba da ima neka metoda za brisanje employee-a? 
        // videti za one lude lockove
        public bool AddNewEmployee(EmployeeCommon.Employee employee)
        {
            try
            {
                var access = new AccessDB();
                access.employees.Add(employee);
                int i = access.SaveChanges(); // srediti ovo, da ne moze u bazu da se doda ono sto vec psotoji  

                if(i > 0)
                    return true;
                return false;
            }
            catch(DbEntityValidationException e)
            {
                foreach(var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach(var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
            }
            return false;
        }


        public Employee GetEmployee(string username)
        {
            using(var access = new AccessDB())
            {
                var employee = from em in access.employees
                               where em.Username.Equals(username)
                               select em;

                return employee.ToList().FirstOrDefault();
            }
        }

        public bool AddNewPartnerCompany(PartnerCompany company)
        {
            try
            {
                var access = new AccessDB();
                access.companies.Add(company);
                int i = access.SaveChanges(); // srediti ovo, da ne moze u bazu da se doda ono sto vec psotoji  

                if(i > 0)
                    return true;
                return false;
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
            return false;
        }
    }
}
