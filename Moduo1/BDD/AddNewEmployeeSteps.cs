using System;
using System.ServiceModel;
using Client;
using EmployeeCommon;
using EmployeeCommon.Data;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class AddNewEmployeeSteps
    {
        private static string svcAddress = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress ea = new EndpointAddress(new Uri(svcAddress));
        private NetTcpBinding binding = new NetTcpBinding();
        private ClientProxy proxy;
        private Employee employee;

        [Given(@"I am logged in as a CEO or HR")]
        public void GivenIAmLoggedInAsACEOOrHR()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            IEmployeeServiceCallback callback = new ClientCallback();
            InstanceContext instanceContext = new InstanceContext(callback);
            proxy = new ClientProxy(instanceContext, binding, ea);
 
            proxy.SignIn("mvujakovic", "123");
        }
        
        [Given(@"I have a form for filling up employee data")]
        public void GivenIHaveAFormForFillingUpEmployeeData()
        {
            employee=new Employee();
        }
        
        [When(@"I enter ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"",")]
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
            Assert.DoesNotThrow(() => proxy.AddNewEmployee(employee));
        }
    }
}
