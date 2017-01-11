﻿using ClientCommon;
using Server.Access;
using System;
using System.Data.Entity;
using System.ServiceModel;
using System.ServiceModel.Description;
using ICommon;
using Server.Logger;
using ClientCommon.Data;

namespace Server
{
    public class Program
    {   
        public static void Main(string[] args)
        {
            // set |DataDirectory|
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = System.IO.Path.GetDirectoryName(executable);
            path = path.Substring(0, path.LastIndexOf("bin")) + "Database";
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            // update database
            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<AccessDB, Configuration>());

            //---EmployeeService host
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/EmployeeService";

            ServiceHost host = new ServiceHost(typeof(EmployeeService));
            host.AddServiceEndpoint(typeof(IEmployeeService), binding, address);
            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            host.Open();
            Console.WriteLine("EmployeeService is started.");
            
            //---OutsourcingService host
            NetTcpBinding bindingOutsourcingService = new NetTcpBinding();
            string addressOutsourcingService = "net.tcp://localhost:9998/OutsourcingService";

            ServiceHost hostOutsourcingService = new ServiceHost(typeof(OutsourcingService));
            hostOutsourcingService.AddServiceEndpoint(typeof(IOutsourcingService), bindingOutsourcingService, addressOutsourcingService);
            hostOutsourcingService.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            hostOutsourcingService.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            hostOutsourcingService.Open();
            Console.WriteLine("OutsourcingService is started.");
            
            Console.WriteLine("Press <enter> to stop service...");

            ////DB Test
            //Employee em1 = new Employee(EmployeeType.CEO, "marko", "markovic", "marko@gmail.com", "mare123", null);
            //EmployeeServiceDatabase.Instance.AddEmployee(em1);

            //OutsourcingService os = new OutsourcingService();
            //os.AskForPartnership("HiringCompany1");

            Console.ReadKey();

            //Notification notif = new Notification(NotificationType.PROJECT_REQUEST, "kompanija", "projekat", "bla");
            //Publisher.Instance.SendNotificationToCEO(notif, null);
            

            Console.ReadKey();
            Console.WriteLine("Press <enter> to exit!");
        }
    }
}
