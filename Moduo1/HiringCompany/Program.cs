﻿using EmployeeCommon;
using HiringCompany.DatabaseAccess;
using HiringCompany.Services;
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
            // mozda ovde da bude setup deo gde cemo citati sve konfiguracione podatke iz fajla

            // set |DataDirectory|
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = System.IO.Path.GetDirectoryName(executable);
            path = path.Substring(0, path.LastIndexOf("bin")) + "Database";
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            // update database
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AccessDB, DBConfiguration>());


            // --------------------- service for clients--------------------------------

            string addressEmployees = "net.tcp://10.1.212.113:9999/EmployeeService"; 
           // string addressEmployees = "net.tcp://localhost:9999/EmployeeService"; //10.1.212.113
            ServiceHost hostEmployees = new ServiceHost(typeof(EmployeeService));
            NetTcpBinding bindingEmployees = new NetTcpBinding();

            bindingEmployees.OpenTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.CloseTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.SendTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.ReceiveTimeout = new TimeSpan(1, 0, 0);
            hostEmployees.AddServiceEndpoint(typeof(IEmployeeService), bindingEmployees, addressEmployees);
            hostEmployees.Open();

            Console.WriteLine("<EmployeesService> service started.");


            //Employee em1 = new Employee("mvujakovic", "123", EmployeeType.CEO, "Miljana", "Vujakovic", "miljana_lo@hotmail.com", 9, 0, 17, 0);
            //Employee em2 = new Employee("zklasnic", "456", EmployeeType.CEO, "Zvezdana", "Klasnic", "zklasnic94@gmail.com", 9, 0, 17, 0);
            //Employee em3 = new Employee("amisic", "789", EmployeeType.PO, "Aleksandra", "Misic", "aleksandra@mail.com", 1, 2, 3, 4);
            //Employee em4 = new Employee("rzekanovic", "112", EmployeeType.SM, "Radislav", "Zekanovic", "radislav@mail.com", 5, 1, 4, 2);
            //Employee em5 = new Employee("pperic", "100", EmployeeType.HR, "Pera", "Peric", "pera@mail.com", 1, 5, 4, 3);

            //HiringCompanyDB.Instance().AddNewEmployee(em1);
            //HiringCompanyDB.Instance().AddNewEmployee(em2);
            //HiringCompanyDB.Instance().AddNewEmployee(em3);
            //HiringCompanyDB.Instance().AddNewEmployee(em4);
            //HiringCompanyDB.Instance().AddNewEmployee(em5);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
            hostEmployees.Close();
        }
    }
}
