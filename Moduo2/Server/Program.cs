using ClientCommon;
using ClientCommon.Data;
using Server.Access;
using System;
using System.Data.Entity;
using System.ServiceModel;
using System.ServiceModel.Description;
using ICommon;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
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

            Console.ReadKey();
            Console.WriteLine("Press <enter> to exit!");
        }
    }
}
