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
    public class ApproveNewProjectSteps
    {
        private static string svcAddress = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress ea = new EndpointAddress(new Uri(svcAddress));
        private NetTcpBinding binding = new NetTcpBinding();
        private ClientProxy proxy;
        private Employee employee;
        private Project selectedProj;
        private string projName;

        [Given(@"I am logged in as a CEO")]
        public void GivenIAmLoggedInAsACEO()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            IEmployeeServiceCallback callback = new ClientCallback();
            InstanceContext instanceContext = new InstanceContext(callback);
            proxy = new ClientProxy(instanceContext, binding, ea);

            proxy.SignIn("mvujakovic", "123");
        }
        
        [Given(@"I have a form for choosing project for approval")]
        public void GivenIHaveAFormForChoosingProjectForApproval()
        {
            selectedProj=new Project();
            projName = "testbdd";
        }
        
        [When(@"I select it and press approve button")]
        public void WhenISelectItAndPressApproveButton()
        {
            selectedProj.Name = projName;
        }
        
        [Then(@"the project should be approved")]
        public void ThenTheProjectShouldBeApproved()
        {
            Assert.DoesNotThrow((() => proxy.ProjectApprovedByCeo(selectedProj)));
        }
    }
}
