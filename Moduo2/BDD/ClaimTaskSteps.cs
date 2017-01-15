using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class ClaimTaskSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        private EmployeeProxy proxy;

        [Given(@"I have a form for entering data for the task")]
        public void GivenIHaveAFormForEnteringDataForTheTask()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }
        
        [When(@"I preess add")]
        public void WhenIPreessAdd()
        {
        }
        
        [Then(@"the task shoul be added to a list of tasks")]
        public void ThenTheTaskShoulBeAddedToAListOfTasks()
        {
            Task task = new Task();
            task.Title = "tassssk";
            task.Description = "desc";

            Assert.DoesNotThrow(() => proxy.TaskClaimed(task));
        }
    }
}
