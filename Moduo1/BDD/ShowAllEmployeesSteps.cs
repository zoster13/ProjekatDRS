using System;
using System.Collections.Generic;
using System.ServiceModel;
using Client;
using EmployeeCommon;
using EmployeeCommon.Data;
using HiringCompany.Services;
using TechTalk.SpecFlow;

namespace BDD
{
    // IZBRISATI OVAJ TEST... nisam sigurna kako da testiram ovo.

    [Binding]
    public class ShowAllEmployeesSteps
    {
        private static string svcAddress = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress ea = new EndpointAddress(new Uri(svcAddress));
        private NetTcpBinding binding = new NetTcpBinding();
       // private ClientProxy proxy;
        private IEmployeeService es;

        private List<Employee> onlineEmployees;

        [When(@"I select Employees tab")]
        public void WhenISelectEmployeesTab()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            IEmployeeServiceCallback callback = new ClientCallback();
            InstanceContext instanceContext = new InstanceContext(callback);

            //proxy = new ClientProxy(instanceContext, binding, ea);
            es=new EmployeeService();
            onlineEmployees=new List<Employee>();
        }
        
        [Then(@"i should see all available colleagues")]
        public void ThenIShouldSeeAllAvailableColleagues()
        {
            
        }
    }
}
