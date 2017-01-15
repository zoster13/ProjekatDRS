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
        // videti gde cuvati ove podatke, mozda na pocetku setup delu iscitati iz fajla sve, neki fajl ili slicno.. 
        //mzftn123fakultet@gmail.com
        //miljanazvezdana123
        private string companyName = "HiringCompany";
        private List<Employee> onlineEmployees;

        // [Key="companyName", Value="ipaddress:port" ]
        private Dictionary<string, string> possiblePartnersAddresses;
        private Dictionary<string, string> partnerCompaniesAddresses;

        private Dictionary<string, OutsorcingCompProxy> connectionChannelsCompanies; //treba zakljucavati
        private Dictionary<string, IEmployeeServiceCallback> connectionChannelsClients; // treba zakljucavati

        private InternalDatabase()
        {
            onlineEmployees = new List<Employee>();

            possiblePartnersAddresses = new Dictionary<string, string>();
            partnerCompaniesAddresses = new Dictionary<string, string>();

            connectionChannelsClients = new Dictionary<string, IEmployeeServiceCallback>();
            connectionChannelsCompanies = new Dictionary<string, OutsorcingCompProxy>();     

            // u fajlu cuvati, i onda iscitati na pocetku programa
            possiblePartnersAddresses.Add("cekic", "10.1.212.114:9998");
            possiblePartnersAddresses.Add("bluc", "10.1.212.114:9998"); // oni ce nam reci podatke

            List<string> pCompaniesName = new List<string>();

            pCompaniesName = HiringCompanyDB.Instance.GetPartnerCompaniesNames();

            //using (var access = new AccessDB())
            //{
            //    pCompaniesName = (from comp in access.Companies
            //                      select comp.Name).ToList();
            //}

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

        // da li ovde streba da stitim u geteru? msm mozda neka druga nit bas tad dodaje nekog employee u online
        //ja mislim da ne treba,jer je ovo memorija za jednog klijenta,a ne moze on brzinom svetlosti dodati 2 Employee
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

        // treba lock ovde?
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
    }
}

