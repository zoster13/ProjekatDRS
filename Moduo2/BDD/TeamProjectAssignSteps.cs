using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class TeamProjectAssignSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        private EmployeeProxy proxy;

        Project proj;
        Team team;

        [Given(@"I have a project and a team")]
        public void GivenIHaveAProjectAndATeam()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }
        
        [When(@"I choose a team and a project")]
        public void WhenIChooseATeamAndAProject()
        {

            team = new Team() { Name = "tim", TeamLeaderEmail = "l", ScrumMasterEmail = "s" };
            proj = new Project() { Name = "projekat", Description = "desc", HiringCompanyName = "kompanija", Team = team};
        }
        
        [Then(@"the project should be assigned to the team")]
        public void ThenTheProjectShouldBeAssignedToTheTeam()
        {
            Assert.DoesNotThrow(() => proxy.ProjectTeamAssign(proj));
        }
    }
}
