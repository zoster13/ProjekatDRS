using ClientCommon;
using ClientCommon.Data;
using Server.Access;
using System;
using System.Data.Entity;
using System.ServiceModel;
using System.ServiceModel.Description;

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

            //EmployeeService host
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/EmployeeService";

            ServiceHost host = new ServiceHost(typeof(EmployeeService));
            host.AddServiceEndpoint(typeof(IEmployeeService), binding, address);
            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            host.Open();

            Console.WriteLine("EmployeeService service is started.");
            Console.WriteLine("Press <enter> to stop service...");

            /*
            //DB Test
            Employee em1 = new Employee(EmployeeType.CEO, "marko", "markovic", "marko@gmail.com", "mare123");
            Employee em2 = new Employee(EmployeeType.HR, "ivan", "ivanic", "ivan@gmail.com", "ivan123");
            Employee em3 = new Employee(EmployeeType.SCRUMMUSTER, "laza", "lazic", "laza@gmail.com", "laza123");
            Employee em4 = new Employee(EmployeeType.TEAMLEADER, "pera", "pera", "pera@gmail.com", "pera123");
            Employee em5 = new Employee(EmployeeType.TEAMLEADER, "sava", "savic", "sava@gmail.com", "sava123");

            EmployeeServiceDatabase.Instance.AddEmployee(em1);
            EmployeeServiceDatabase.Instance.AddEmployee(em2);
            EmployeeServiceDatabase.Instance.AddEmployee(em3);
            EmployeeServiceDatabase.Instance.AddEmployee(em4);
            EmployeeServiceDatabase.Instance.AddEmployee(em5);
            */

            Console.ReadKey();
            Console.WriteLine("Press <enter> to exit!");
        }
    }
}
