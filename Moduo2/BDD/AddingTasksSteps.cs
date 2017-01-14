using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class AddingTasksSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        public EmployeeProxy proxy;

        [Given(@"I have methods for defining tasks")]
        public void GivenIHaveMethodsForDefiningTasks()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }
        
        [When(@"I enter data for a task and press button")]
        public void WhenIEnterDataForATaskAndPressButton()
        {
            //ScenarioContext.Current.Pending();
        }

        [Then(@"it is added to a list of tasks")]
        public void ThenItIsAddedToAListOfTasks()
        {
            Task task = new Task();
            task.Title = "task";
            task.Description = "task desc";

            Assert.DoesNotThrow(() => proxy.AddTask(task));
        }
    }
}
