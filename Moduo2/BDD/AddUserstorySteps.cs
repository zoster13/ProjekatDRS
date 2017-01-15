using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class AddUserstorySteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        private EmployeeProxy proxy;

        [Given(@"I have a form for creating user stories")]
        public void GivenIHaveAFormForCreatingUserStories()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }
        
        [When(@"I press add")]
        public void WhenIPressAdd()
        {
        }
        
        [Then(@"the new story is added to the list")]
        public void ThenTheNewStoryIsAddedToTheList()
        {
            UserStory us = new UserStory();
            us.Title = "uss1";
            us.Description = "us1 desc";
            us.AcceptanceCriteria = "acc criteria";

            Assert.DoesNotThrow(() => proxy.AddUserStory(us, "projekat"));
        }
    }
}
