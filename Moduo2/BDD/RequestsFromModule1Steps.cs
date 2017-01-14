using System;
using TechTalk.SpecFlow;
using ClientCommon;
using Client;
using System.ServiceModel;
using NUnit.Framework;
using ClientCommon.Data;

namespace BDD
{
    [Binding]
    public class RequestsFromModule1Steps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        public EmployeeProxy proxy;

        [Given(@"I have the service")]
        public void GivenIHaveTheService()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());

            //ScenarioContext.Current.Pending();
        }
        
        [When(@"I get a notification for partnership and accept it")]
        public void WhenIGetANotificationForPartnershipAndAcceptIt()
        {
            //ScenarioContext.Current.Pending();
        }

        [Then(@"the partnership answer is sent to the service")]
        public void ThenThePartnershipAnswerIsSentToTheService()
        {
            Assert.DoesNotThrow(() => proxy.ResponseToPartnershipRequest(true, "kompanija"));
            //ScenarioContext.Current.Pending();
        }
        
        [When(@"I get a notification for a project and accept it")]
        public void WhenIGetANotificationForAProjectAndAcceptIt()
        {
            //ScenarioContext.Current.Pending();
        }
        
        [Then(@"the ptoject answer is sent to the service")]
        public void ThenThePtojectAnswerIsSentToTheService()
        {
            Project proj = new Project();
            proj.Name = "projekat";
            proj.Description = "projekat opis";
            proj.HiringCompanyName = "kompanija";

            Assert.DoesNotThrow(() => proxy.ResponseToProjectRequest(true, proj));
            //ScenarioContext.Current.Pending();
        }
    }
}
