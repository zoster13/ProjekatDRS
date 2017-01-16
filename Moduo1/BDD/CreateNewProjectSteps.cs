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
    public class CreateNewProjectSteps
    {
        private static string svcAddress = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress ea = new EndpointAddress(new Uri(svcAddress));
        private NetTcpBinding binding = new NetTcpBinding();
        private ClientProxy proxy;

        private Project newProj;
        private string projectName;

        [Given(@"I have a form for creating Projects")]
        public void GivenIHaveAFormForCreatingProjects()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            IEmployeeServiceCallback callback = new ClientCallback();
            InstanceContext instanceContext = new InstanceContext(callback);
            proxy = new ClientProxy(instanceContext, binding, ea);
        }
        
        [Given(@"I have fill it with data")]
        public void GivenIHaveFillItWithData()
        {
            projectName = "testbdd";
            newProj=new Project();
        }
        
        [When(@"I press create button")]
        public void WhenIPressCreateButton()
        {
            newProj.Name = projectName;
        }
        
        [Then(@"the proejct should be created")]
        public void ThenTheProejctShouldBeCreated()
        {
            Assert.DoesNotThrow((() => proxy.CreateNewProject(newProj)));
        }
    }
}
