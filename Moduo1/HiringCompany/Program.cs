using EmployeeCommon;
using HiringCompany.EmployeesMng;
using System;
using System.Collections.Generic;
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

            string addressEmployees = "net.tcp://localhost:9999/EmployeeService";
            ServiceHost hostEmployees = new ServiceHost(typeof(EmployeeService));
            NetTcpBinding bindingEmployees = new NetTcpBinding();

            bindingEmployees.OpenTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.CloseTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.SendTimeout = new TimeSpan(1, 0, 0);
            bindingEmployees.ReceiveTimeout = new TimeSpan(1, 0, 0);
            hostEmployees.AddServiceEndpoint(typeof(IEmployeeService), bindingEmployees, addressEmployees);
            hostEmployees.Open();

            Console.WriteLine("<EmployeesService> service started.");

            Console.ReadKey(true);
            hostEmployees.Close();
        }
    }
}
