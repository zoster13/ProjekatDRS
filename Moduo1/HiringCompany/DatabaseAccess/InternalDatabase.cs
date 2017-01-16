using EmployeeCommon;
using EmployeeCommon.Data;
using HiringCompany.DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany.DatabaseAccess
{
    public class InternalDatabase
    {
        private static InternalDatabase instance;

        //  lock objects for synchronizing access to in-memory data
        private object onlineEmployees_lock = new object();
        private object projectsForApproval_lock = new object();
        private object partnerCompaniesAddresses_lock = new object();

        // lock objects for synchronizing access to data about active communication channels
        private object channelsCompanies_lock = new object();
        private object channelsClients_lock = new object();

        // in-memory data
        // mail: mzftn123fakultet@gmail.com
        // pass: miljanazvezdana123
        private string companyName = "HiringCompany";
        private List<Employee> onlineEmployees;

        // [Key="companyName", Value="ipaddress:port" ]
        private Dictionary<string, string> possiblePartnersAddresses;
        private Dictionary<string, string> partnerCompaniesAddresses;

        private Dictionary<string, OutsorcingCompProxy> connectionChannelsCompanies;
        private Dictionary<string, IEmployeeServiceCallback> connectionChannelsClients;

        private InternalDatabase()
        {
            onlineEmployees = new List<Employee>();

            possiblePartnersAddresses = new Dictionary<string, string>();
            partnerCompaniesAddresses = new Dictionary<string, string>();

            connectionChannelsClients = new Dictionary<string, IEmployeeServiceCallback>();
            connectionChannelsCompanies = new Dictionary<string, OutsorcingCompProxy>();

            // oni ce nam reci podatke
            possiblePartnersAddresses.Add("cekic", "10.1.212.114:9998");
            possiblePartnersAddresses.Add("bluc", "10.1.212.114:9998"); 
            possiblePartnersAddresses.Add("prva", "10.1.212.114:9998");
            possiblePartnersAddresses.Add("druga", "10.1.212.114:9998");
            possiblePartnersAddresses.Add("treca", "10.1.212.114:9998");

            List<string> pCompaniesName = new List<string>();

            pCompaniesName = HiringCompanyDB.Instance.GetPartnerCompaniesNames();

            foreach (string cName in pCompaniesName)
            {
                partnerCompaniesAddresses.Add(cName, possiblePartnersAddresses[cName]);
                possiblePartnersAddresses.Remove(cName);
            }
        }

        public static InternalDatabase Instance()
        {
            if (instance == null)
            {
                instance = new InternalDatabase();
            }
            return instance;
        }

        public object OnlineEmployees_lock
        {
            get { return onlineEmployees_lock; }
            set { onlineEmployees_lock = value; }
        }

        public object ProjectsForApproval_lock
        {
            get { return projectsForApproval_lock; }
            set { projectsForApproval_lock = value; }
        }

        public object PartnerCompaniesAddresses_lock
        {
            get { return partnerCompaniesAddresses_lock; }
            set { partnerCompaniesAddresses_lock = value; }
        }

        public object ChannelsCompanies_lock
        {
            get { return channelsCompanies_lock; }
            set { channelsCompanies_lock = value; }
        }

        public object ChannelsClients_lock
        {
            get { return channelsClients_lock; }
            set { channelsClients_lock = value; }
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

        public bool EditOnlineEmployeeData(string username, string name, string surname, string email, string password)
        {
            string messageToLog = string.Empty;
            bool retval = false;
            lock (OnlineEmployees_lock)
            {
                Employee em = OnlineEmployees.Find(e => e.Username.Equals(username));
                if (em != null)
                {
                    em.Name = name != "" ? name : em.Name;
                    em.Surname = surname != "" ? surname : em.Surname;
                    em.Email = email != "" ? email : em.Email;
                    em.Password = password != "" ? password : em.Password;
                    messageToLog = "updated employee data in OnlineEmployees list.";
                    Program.Logger.Info(messageToLog);
                    retval = true;
                }            
            }
            return retval;
        }
        public bool EditWorkingHoursForOnlineEm(string username, int beginH, int beginM, int endH, int endM)
        {
            string messageToLog = string.Empty;
            bool retval = false;
            lock (OnlineEmployees_lock)
            {
                Employee em = OnlineEmployees.Find(e => e.Username.Equals(username));
                if (em != null)
                {
                    em.StartHour = beginH;
                    em.StartMinute = beginM;
                    em.EndHour = endH;
                    em.EndMinute = endM;
                    messageToLog = "updated working hours data in OnlineEmployees list.";
                    retval = true;
                }
            }
            return retval;
        }

        public bool EditOnlineEmployeeType(string username, EmployeeType type)
        {
            string messageToLog = string.Empty;
            bool retVal = false;
            lock (OnlineEmployees_lock)
            {
                Employee em = OnlineEmployees.Find(e => e.Username.Equals(username));
                if (em != null)
                {
                    em.Type = type;
                    messageToLog = "employee type changed in OnlineEmployees list";
                    Program.Logger.Info(messageToLog);
                    retVal = true;
                }
            }
            return retVal;
        }
    }
}

