using System;
using TechTalk.SpecFlow;
using Client;
using ClientCommon;
using System.ServiceModel;
using NUnit.Framework;
using ClientCommon.Data;

namespace BDD
{
    [Binding]
    public class TeamProjectAssignSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        private EmployeeProxy proxy;

        [Given(@"I have the service methods for assigning")]
        public void GivenIHaveTheServiceMethodsForAssigning()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());

            //ScenarioContext.Current.Pending();
        }
        
        [When(@"I choose a team and press button")]
        public void WhenIChooseATeamAndPressButton()
        {
            //ScenarioContext.Current.Pending();
        }
        
        [Then(@"the project is sent to the team leader")]
        public void ThenTheProjectIsSentToTheTeamLeader()
        {
            Project proj = new Project();
            proj.Name = "projekat";
            proj.Description = "projekat opis";
            proj.HiringCompanyName = "kompanija";

            Assert.DoesNotThrow(() => proxy.ProjectTeamAssign(proj));
        }
    }
}
