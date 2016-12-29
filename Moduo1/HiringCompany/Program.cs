using EmployeeCommon;
using HiringCompany.DatabaseAccess;
using HiringCompany.EmployeesMng;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Hiring Company";
            // to do: initialization data and background setup...

            // set |DataDirectory|
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = System.IO.Path.GetDirectoryName(executable);
            path = path.Substring(0, path.LastIndexOf("bin")) + "Database";
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            // update database
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AccessDB, DBConfiguration>());



            string addressEmployees = "net.tcp://10.1.212.113:9999/EmployeeService";
            ServiceHost hostEmployees = new ServiceHost(typeof(EmployeeService));
            NetTcpBinding bindingEmployees = new NetTcpBinding();

            bindingEmployees.OpenTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.CloseTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.SendTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.ReceiveTimeout = new TimeSpan(1, 0, 0);
            hostEmployees.AddServiceEndpoint(typeof(IEmployeeService), bindingEmployees, addressEmployees);
            hostEmployees.Open();

            Console.WriteLine("<EmployeesService> service started.");


           // Employee em1 = new Employee("mvujakovic", "123", EmployeeType.CEO, "Miljana", "Vujakovic", "miljana@mail.com",1,2,2,3);
            //Employee em2 = new Employee("zklasnic", "456", EmployeeType.CEO, "Zvezdana", "Klasnic", "zvezdana@mail.com");
            //Employee em3 = new Employee("amisic", "789", EmployeeType.PO, "Aleksandra", "Misic", "aleksandra@mail.com");
            //Employee em4 = new Employee("rzekanovic", "112", EmployeeType.SM, "Radislav", "Zekanovic", "radislav@mail.com");
            //Employee em5 = new Employee("pperic", "100", EmployeeType.HR, "Pera", "Peric", "pera@mail.com");

            //HiringCompanyDB.Instance().AddNewEmployee(em1);
            //HiringCompanyDB.DBInstance.AddNewEmployee(em2);
            //HiringCompanyDB.DBInstance.AddNewEmployee(em3);
            //HiringCompanyDB.DBInstance.AddNewEmployee(em4);
            //HiringCompanyDB.DBInstance.AddNewEmployee(em5);


            Console.ReadKey(true);
            hostEmployees.Close();
        }
    }
}
