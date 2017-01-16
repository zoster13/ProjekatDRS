using EmployeeCommon;
using EmployeeCommon.Data;
using HiringCompany.DatabaseAccess;
using HiringCompany.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using HiringCompany.Logger;
using ICommon;

namespace HiringCompany
{


    public class Program
    {
        public static readonly log4net.ILog Logger = LogHelper.GetLogger();

        private static void Main(string[] args)
        {
            Console.Title = "Hiring Company";   

            // set |DataDirectory|
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = System.IO.Path.GetDirectoryName(executable);
            path = path.Substring(0, path.LastIndexOf("bin")) + "Database";
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            // update database
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AccessDB, DBConfiguration>());


            // --------------------- service for clients--------------------------------
           
            string addressEmployees = "net.tcp://10.1.212.113:9999/EmployeeService"; 
            //string addressEmployees = "net.tcp://localhost:9999/EmployeeService"; 
            ServiceHost hostEmployees = new ServiceHost(typeof(EmployeeService));
            NetTcpBinding bindingEmployees = new NetTcpBinding();

            bindingEmployees.OpenTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.CloseTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.SendTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.ReceiveTimeout = new TimeSpan(1, 0, 0);
            hostEmployees.AddServiceEndpoint(typeof(IEmployeeService), bindingEmployees, addressEmployees);
            hostEmployees.Open();

            Console.WriteLine("<EmployeesService> service started.");


            // --------------------- service for outsorcing companies--------------------------------

            string addressCompanies = "net.tcp://10.1.212.113:9998/HiringService"; 
            //string addressCompanies = "net.tcp://localhost:9998/HiringService"; 
            ServiceHost hostCompanies = new ServiceHost(typeof(HiringService));
            NetTcpBinding bindingCompanies = new NetTcpBinding();

            bindingCompanies.OpenTimeout = new TimeSpan(1, 0, 0);
            bindingCompanies.CloseTimeout = new TimeSpan(1, 0, 0);
            bindingCompanies.SendTimeout = new TimeSpan(1, 0, 0);
            bindingCompanies.ReceiveTimeout = new TimeSpan(1, 0, 0);
            hostCompanies.AddServiceEndpoint(typeof(IHiringService), bindingCompanies, addressCompanies);
            hostCompanies.Open();

            Console.WriteLine("<HiringService> service started.");

            //Console.ReadKey(true);

            //HiringService hs = new HiringService();
            //List<UserStoryCommon> ustories = new List<UserStoryCommon>();
            //UserStoryCommon us = new UserStoryCommon("proba", "hahah", "beeee", false);
            //UserStoryCommon us1 = new UserStoryCommon("druga", "desc", "gee", false);
            //ustories.Add(us);
            //ustories.Add(us1);
            //hs.SendUserStoriesToHiringCompany(ustories, "proradiiiii");
            //hs.SendClosedUserStory("prvi", "proba");
            //hs.SendClosedUserStory("prvi", "druga");



            //Employee em1 = new Employee("mvujakovic", "123", EmployeeType.CEO, "Miljana", "Vujakovic", "miljana_lo@hotmail.com", 9, 0, 17, 0) { DatePasswordChanged = DateTime.Now };
            //Employee em2 = new Employee("zklasnic", "456", EmployeeType.CEO, "Zvezdana", "Klasnic", "zklasnic94@gmail.com", 9, 0, 17, 0) { DatePasswordChanged = DateTime.Now };
            //Employee em3 = new Employee("amisic", "789", EmployeeType.PO, "Aleksandra", "Misic", "miljana_lo@hotmail.com", 1, 2, 3, 4) { DatePasswordChanged = DateTime.Now };
            //Employee em4 = new Employee("rzekanovic", "112", EmployeeType.SM, "Radislav", "Zekanovic", "zklasnic94@gmail.com", 5, 1, 4, 2) { DatePasswordChanged = DateTime.Now };
            //Employee em5 = new Employee("pperic", "100", EmployeeType.HR, "Pera", "Peric", "zklasnic94@gmail.com", 1, 5, 4, 3) { DatePasswordChanged = DateTime.Now };

            //HiringCompanyDB.Instance.AddNewEmployee(em1);
            //HiringCompanyDB.Instance.AddNewEmployee(em2);
            //HiringCompanyDB.Instance.AddNewEmployee(em3);
            //HiringCompanyDB.Instance.AddNewEmployee(em4);
            //HiringCompanyDB.Instance.AddNewEmployee(em5);


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
            hostEmployees.Close();
            hostCompanies.Close();
        }
    }
}
