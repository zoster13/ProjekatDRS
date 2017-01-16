using System;
using System.ServiceModel;
using Client;
using EmployeeCommon;
using EmployeeCommon.Data;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class SetWorkingHoursSteps
    {
        private static string svcAddress = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress ea = new EndpointAddress(new Uri(svcAddress));
        private NetTcpBinding binding = new NetTcpBinding();
        private ClientProxy proxy;
        private Employee employee;
        IEmployeeServiceCallback callback;
        InstanceContext instanceContext;

        [Given(@"I have form for re-enter my data")]
        public void GivenIHaveFormForRe_EnterMyData()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);   
            employee = new Employee();
        }
        
        [Given(@"I am Logged in")]
        public void GivenIAmLoggedIn()
        {
            callback = new ClientCallback();
            instanceContext = new InstanceContext(callback);
            proxy = new ClientProxy(instanceContext, binding, ea);
            proxy.SignIn("mvujakovic", "123");
        }
        
        [When(@"I press add save changes button")]
        public void WhenIPressAddSaveChangesButton()
        {
            // premenuti feature, da ima prametre...
        }
        
        [Then(@"my working hours should be changed")]
        public void ThenMyWorkingHoursShouldBeChanged()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
