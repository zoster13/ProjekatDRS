using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ClientCommon;
using ClientCommon.Data;

namespace Client
{
    public class EmployeeProxy : ChannelFactory<IEmployeeService>, IEmployeeService, IDisposable
    {
        IEmployeeService factory;

        public EmployeeProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public Employee LogIn(string email, string password)
        {
            return factory.LogIn(email, password);
        }
        
        public List<Employee> GetAllEmployees()
        {
            var res = factory.GetAllEmployees();

            return res;
        }
        public bool LogOut(string email)
        {
            throw new NotImplementedException();
        }
    }
}
