using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class RequestForProjectSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        private EmployeeProxy proxy;

        [Given(@"I have a notification for project")]
        public void GivenIHaveANotificationForProject()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }
        
        [When(@"I accept the request for project")]
        public void WhenIAcceptTheRequestForProject()
        {
            //ScenarioContext.Current.Pending();
        }

        [Then(@"the response for project  is sent to the requesting company")]
        public void ThenTheResponseForProjectIsSentToTheRequestingCompany()
        {
            Project proj = new Project();
            proj.Name = "projekat";
            proj.Description = "proj desc";
            proj.HiringCompanyName = "kompanija";

            Assert.DoesNotThrow(() => proxy.ResponseToProjectRequest(true, proj));
        }
    }
}
