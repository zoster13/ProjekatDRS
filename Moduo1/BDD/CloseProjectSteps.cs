using System;
using System.ServiceModel;
using Client;
using EmployeeCommon;
using EmployeeCommon.Data;
using TechTalk.SpecFlow;
using NUnit.Framework;
namespace BDD
{
    [Binding]
    public class CloseProjectSteps
    {
        private static string svcAddress = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress ea = new EndpointAddress(new Uri(svcAddress));
        private NetTcpBinding binding = new NetTcpBinding();
        private ClientProxy proxy;
        private Employee employee;
        private Project selectedProj;
        private string projName;
        private string po;

        [Given(@"I am logged in as a PO")]
        public void GivenIAmLoggedInAsAPO()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            IEmployeeServiceCallback callback = new ClientCallback();
            InstanceContext instanceContext = new InstanceContext(callback);
            proxy = new ClientProxy(instanceContext, binding, ea);

            po = "rzekanovic";
            proxy.SignIn(po, "112");
        }
        
        [Given(@"I have a form for choosing project for closing")]
        public void GivenIHaveAFormForChoosingProjectForClosing()
        {
            selectedProj = new Project();
          
        }
        
        [Given(@"I have select it")]
        public void GivenIHaveSelectIt()
        {
            projName = "testbdd";
        }
        
        [When(@"I press close button")]
        public void WhenIPressCloseButton()
        {
            selectedProj.Name = projName;
            selectedProj.ProductOwner = po;
        }

        [Then(@"the project should be changed")]
        public void ThenTheProjectShouldBeChanged()
        {
            Assert.DoesNotThrow((() => proxy.CloseProject(selectedProj.Name)));
        }
    }
}
