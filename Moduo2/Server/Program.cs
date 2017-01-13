using ClientCommon;
using Server.Access;
using System;
using System.Data.Entity;
using System.ServiceModel;
using System.ServiceModel.Description;
using ICommon;
using ClientCommon.Data;
using System.Collections.Generic;

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
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);
            string address = "net.tcp://localhost:9999/EmployeeService";

            ServiceHost host = new ServiceHost(typeof(EmployeeService));
            host.AddServiceEndpoint(typeof(IEmployeeService), binding, address);
            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            host.Open();
            Console.WriteLine("EmployeeService is started.");
            
            //---OutsourcingService host
            NetTcpBinding bindingOutsourcingService = new NetTcpBinding();
            bindingOutsourcingService.OpenTimeout = new TimeSpan(1, 0, 0);
            bindingOutsourcingService.CloseTimeout = new TimeSpan(1, 0, 0);
            bindingOutsourcingService.SendTimeout = new TimeSpan(1, 0, 0);
            bindingOutsourcingService.ReceiveTimeout = new TimeSpan(1, 0, 0);
            string addressOutsourcingService = "net.tcp://localhost:9998/OutsourcingService";

            ServiceHost hostOutsourcingService = new ServiceHost(typeof(OutsourcingService));
            hostOutsourcingService.AddServiceEndpoint(typeof(IOutsourcingService), bindingOutsourcingService, addressOutsourcingService);
            hostOutsourcingService.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            hostOutsourcingService.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            hostOutsourcingService.Open();
            Console.WriteLine("OutsourcingService is started.");
            
            Console.WriteLine("Press <enter> to stop service...");

            ////DB Test
            ////Employee em1 = new Employee(EmployeeType.CEO, "marko", "markovic", "marko@gmail.com", "mare123", null);
            ////EmployeeServiceDatabase.Instance.AddEmployee(em1);

            //OutsourcingService os = new OutsourcingService();
            //os.AskForPartnership("HiringCompany1");

            //Console.ReadKey();

            //Notification notif = new Notification(NotificationType.PROJECT_REQUEST, "kompanija", "projekat5", "bla");
            //Publisher.Instance.SendNotificationToCEO(notif);

            //Console.ReadKey();

            //Notification notif1 = new Notification(NotificationType.PROJECT_REQUEST, "kompanija", "projekat4", "bla");
            //Publisher.Instance.SendNotificationToCEO(notif1);

            //Console.ReadKey();

            //UserStoryCommon us1 = new UserStoryCommon();
            //us1.Title = "us1";
            //us1.Description = "prvi user story";
            //us1.IsAccepted = true;
            //UserStoryCommon us2 = new UserStoryCommon();
            //us2.Title = "us2";
            //us2.Description = "drugi user story";
            //us2.IsAccepted = false;

            //List<UserStoryCommon> usl = new List<UserStoryCommon>();
            //usl.Add(us1);
            //usl.Add(us2);
            //Publisher.Instance.ReceiveUserStoriesCallback(usl, "projekat5");

            Console.ReadKey();
            Console.WriteLine("Press <enter> to exit!");
        }
    }
}
