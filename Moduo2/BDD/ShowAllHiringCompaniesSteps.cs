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

        [Given(@"I have a table for showing all hiring companies")]
        public void GivenIHaveATableForShowingAllHiringCompanies()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }
        
        [When(@"I llog in")]
        public void WhenILlogIn()
        {
        }
        
        [Then(@"all companies are listed in the table")]
        public void ThenAllCompaniesAreListedInTheTable()
        {
            Assert.DoesNotThrow(() => proxy.GetAllHiringCompanies());
        }
    }
}
