using System;
using System.ServiceModel;
using Client;
using EmployeeCommon;
using EmployeeCommon.Data;
using NUnit.Core;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class EditEmployeeDataSteps
    {
        private static string svcAddress = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress ea = new EndpointAddress(new Uri(svcAddress));
        private NetTcpBinding binding = new NetTcpBinding();
        private ClientProxy proxy;
        private Employee employee;

        [Given(@"I a form for editing data")]
        public void GivenIAFormForEditingData()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            IEmployeeServiceCallback callback = new ClientCallback();
            InstanceContext instanceContext = new InstanceContext(callback);
            proxy = new ClientProxy(instanceContext, binding, ea);

            employee=new Employee();
        }
        
        [When(@"I change ""(.*)"", ""(.*)"", ""(.*)"",")]
        public void WhenIChange(string p0, string p1, int p2)
        {
            employee.Name = p0;
            employee.Surname = p1;
            employee.Password = p2.ToString();
        }
        
        [Then(@"the changes are displayed")]
        public void ThenTheChangesAreDisplayed()
        {
            Assert.DoesNotThrow(()=>proxy.ChangeEmployeeData(employee.Username,employee.Name,employee.Surname,employee.Email,employee.Password));
        }
    }
}
