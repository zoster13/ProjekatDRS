using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class AddEmployeeSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        private EmployeeProxy proxy;

        private Employee employee;

        [Given(@"I a form for entering data")]
        public void GivenIAFormForEnteringData()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());

            employee = new Employee();
        }
        
        [When(@"I enter (.*), (.*), (.*), (.*),")]
        public void WhenIEnter(string p0, string p1, string p2, string p3)
        {
            employee.Name = p0;
            employee.Surname = p1;
            employee.Email = p2;
            employee.Password = p3;
        }
        
        [Then(@"the new employee should be added")]
        public void ThenTheNewEmployeeShouldBeAdded()
        {
            Assert.DoesNotThrow(() => proxy.AddEmployee(employee));
        }
    }
}
