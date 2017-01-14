using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class EditEmployeeDataSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        private EmployeeProxy proxy;

        [Given(@"I have a form for entering new data")]
        public void GivenIHaveAFormForEnteringNewData()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }
        
        [When(@"I enter data and presss button")]
        public void WhenIEnterDataAndPresssButton()
        {
            //ScenarioContext.Current.Pending();
        }
        
        [Then(@"my data is changed")]
        public void ThenMyDataIsChanged()
        {
            Employee em = new Employee();
            em.Name = "nenad1";
            em.Surname = "nenadovic1";
            em.Email = "nenad@gmail.com1";
            em.Password = "nenad1231";

            Assert.DoesNotThrow(() => proxy.EditEmployeeData(em));
        }
    }
}
