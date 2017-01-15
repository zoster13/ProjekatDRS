using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class LogInSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        private EmployeeProxy proxy;

        private string email;
        private string password;

        [Given(@"I have form to log in")]
        public void GivenIHaveFormToLogIn()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"I enter ""(.*)"" and ""(.*)""")]
        public void WhenIEnterAnd(string p0, string p1)
        {
            email = p0;
            password = p1;
        }
        
        [Then(@"I should be logged in")]
        public void ThenIShouldBeLoggedIn()
        {
            Assert.DoesNotThrow(() => proxy.LogIn(email, password));
        }
    }
}
