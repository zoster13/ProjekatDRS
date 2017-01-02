using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClientCommon;
using ClientCommon.Data;

namespace Client
{
    public class EmployeeProxy : DuplexChannelFactory<IEmployeeService>, IEmployeeService, IDisposable
    {
        IEmployeeService factory;
        CallbackMethods callbackMethods = null;

        public EmployeeProxy(NetTcpBinding binding, EndpointAddress epAddress, CallbackMethods callbackMethods) : base(callbackMethods, binding, epAddress)
        {
            this.callbackMethods = callbackMethods;
            factory = this.CreateChannel();
        }

        public void LogIn(string email, string password)
        {
            try
            {
                factory.LogIn(email, password);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogIn: {0}", e.Message);
            }
        }

        public void LogOut(string email)
        {
            try
            {
                factory.LogOut(email);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogOut: {0}", e.Message);
            }
        }

        public List<Employee> GetAllEmployees()
        {
            try
            {
                return factory.GetAllEmployees();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetAllEmployees: {0}", e.Message);
                return null;
            }
        }

        public List<Team> GetAllTeams()
        {
            try
            {
                return factory.GetAllTeams();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetAllTeams: {0}", e.Message);
                return null;
            }
        }

        public List<HiringCompany> GetAllHiringCompanies()
        {
            try
            {
                return factory.GetAllHiringCompanies();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetAllHiringCompanies: {0}", e.Message);
                return null;
            }
        }
    }
}
