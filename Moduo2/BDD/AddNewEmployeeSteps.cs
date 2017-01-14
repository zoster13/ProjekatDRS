using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class AddNewEmployeeSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        public EmployeeProxy proxy;

        [Given(@"I form for entering employee data")]
        public void GivenIFormForEnteringEmployeeData()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }
        
        [When(@"I enter data and press button")]
        public void WhenIEnterDataAndPressButton()
        {
            //ScenarioContext.Current.Pending();
        }

        [Then(@"ta new employee is added")]
        public void ThenTaNewEmployeeIsAdded()
        {
            Employee em = new Employee();
            em.Name = "nenad";
            em.Surname = "nenadovic";
            em.Email = "nenad@gmail.com";
            em.Password = "nenad123";

            Assert.DoesNotThrow(() => proxy.AddEmployee(em));
        }
    }
}
