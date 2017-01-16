using System;
using System.ServiceModel;
using Client;
using NUnit.Core;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class LogInSteps
    {
        private static string svcAddress = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress ea = new EndpointAddress(new Uri(svcAddress));
        private NetTcpBinding binding = new NetTcpBinding();
        private ClientProxy proxy;

        private string username;
        private string password;

        private bool result;

        [Given(@"I have form to log in")]
        public void GivenIHaveFormToLogIn()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I enter valid ""(.*)"" and ""(.*)""")]
        public void WhenIEnterValidAnd( string p0, int p1 )
        {
            username = p0;
            password = p1.ToString();
        }

        [When(@"I enter wrong ""(.*)"" or ""(.*)""")]
        public void WhenIEnterWrongOr( string p0, string p1 )
        {
            username = p0;
            password = p1;
        }

        [Then(@"I should be logged in successfully")]
        public void ThenIShouldBeLoggedInSuccessfully()
        {
            result = proxy.SignIn(username, password);
            NUnitFramework.Assert.AreEqual(true, result);
        }

        [Then(@"I should be warned")]
        public void ThenIShouldBeWarned()
        {
            result = proxy.SignIn(username, password);
            NUnitFramework.Assert.AreEqual(false, result);
        }
    }
}
