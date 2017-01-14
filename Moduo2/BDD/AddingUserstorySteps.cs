using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class AddingUserstorySteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        public EmployeeProxy proxy;

        [Given(@"I have service methods for defining user stories")]
        public void GivenIHaveServiceMethodsForDefiningUserStories()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());

            //ScenarioContext.Current.Pending();
        }
        
        [When(@"I enter userstory data")]
        public void WhenIEnterUserstoryData()
        {
            //ScenarioContext.Current.Pending();
        }

        [Then(@"the user story is added")]
        public void ThenTheUserStoryIsAdded()
        {
            UserStory us = new UserStory();
            us.Title = "us1";
            us.Description = "us1 desc";
            us.AcceptanceCriteria = "acc criteria";
            Assert.DoesNotThrow(() => proxy.AddUserStory(us, "projekat"));
        }
    }
}
