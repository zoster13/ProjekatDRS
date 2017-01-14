using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class ShowAllHiringCompaniesSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        private EmployeeProxy proxy;

        [Given(@"I have a space for all hiring companies")]
        public void GivenIHaveASpaceForAllHiringCompanies()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }
        
        [When(@"I log inn")]
        public void WhenILogInn()
        {
            //ScenarioContext.Current.Pending();
        }
        
        [Then(@"I can see all hiring companiesin the space")]
        public void ThenICanSeeAllHiringCompaniesinTheSpace()
        {
            Assert.DoesNotThrow(() => proxy.GetAllHiringCompanies());
        }
    }
}
