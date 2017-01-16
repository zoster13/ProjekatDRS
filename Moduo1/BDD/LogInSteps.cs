using System;
using System.ServiceModel;
using Client;
using EmployeeCommon;
using NUnit.Framework;
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
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            IEmployeeServiceCallback callback = new ClientCallback();
            InstanceContext instanceContext = new InstanceContext(callback);
            proxy = new ClientProxy(instanceContext, binding, ea);

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
            Assert.DoesNotThrow(() => proxy.SignIn(username, password));
        }

        [Then(@"I should be warned")]
        public void ThenIShouldBeWarned()
        {
            result = proxy.SignIn(username, password);
            Assert.AreEqual(false, result);
        }
    }
}
