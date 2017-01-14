using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class ShowAllTeamsSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        public EmployeeProxy proxy;

        [Given(@"I have a space for all teams")]
        public void GivenIHaveASpaceForAllTeams()
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
            //ScenarioContext.Current.Pending();
        }
        
        [Then(@"I can see all teams in the space")]
        public void ThenICanSeeAllTeamsInTheSpace()
        {
            Assert.DoesNotThrow(() => proxy.GetAllTeams());
        }
    }
}
