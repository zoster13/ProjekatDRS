using System;
using System.ServiceModel;
using Client;
using NUnit.Core;
using TechTalk.SpecFlow;
using NUnit.Framework;
using EmployeeCommon;

namespace BDD
{
    [Binding]
    public class LogOutSteps
    {
        private static string svcAddress = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress ea = new EndpointAddress(new Uri(svcAddress));
        private NetTcpBinding binding = new NetTcpBinding();
        private ClientProxy proxy;

        private bool result;
        private string username;
        private string password;

        [Given(@"I am logged in")]
        public void GivenIAmLoggedIn()
        {
            IEmployeeServiceCallback callback = new ClientCallback();
            InstanceContext instanceContext = new InstanceContext(callback);
            proxy = new ClientProxy(instanceContext, binding, ea);
            proxy.SignIn("mvujakovic", "123");
        }
        
        [When(@"I press x button")]
        public void WhenIPressXButton()
        {
            username = "mvujakovic";           
        }
        
        [Then(@"I should be successfully logged out")]
        public void ThenIShouldBeSuccessfullyLoggedOut()
        {
            Assert.DoesNotThrow( () => proxy.SignOut(username));
        }
    }
}
