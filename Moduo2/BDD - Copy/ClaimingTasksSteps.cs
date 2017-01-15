using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;


namespace BDD
{
    [Binding]
    public class ClaimingTasksSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        private EmployeeProxy proxy;

        [Given(@"I have methods for claiming tasks")]
        public void GivenIHaveMethodsForClaimingTasks()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }
        
        [When(@"I choose a task and press button")]
        public void WhenIChooseATaskAndPressButton()
        {
            //ScenarioContext.Current.Pending();
        }
        
        [Then(@"the task in the list of tasks is market as claimed")]
        public void ThenTheTaskInTheListOfTasksIsMarketAsClaimed()
        {
            Task task = new Task();
            task.Title = "task";
            task.Description = "task desc";

            Assert.DoesNotThrow(() => proxy.TaskClaimed(task));
        }
    }
}
