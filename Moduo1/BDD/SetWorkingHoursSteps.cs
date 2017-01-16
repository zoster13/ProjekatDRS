using System;
using System.ServiceModel;
using Client;
using EmployeeCommon;
using EmployeeCommon.Data;
using NUnit.Framework;
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
        private int sh, eh, sm, em;
        private string username;

        [Given(@"I have form for re-enter my data")]
        public void GivenIHaveFormForRe_EnterMyData()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);   
            employee = new Employee();
            username = "mvujakovic";
        }
        
        [Given(@"I am Logged in")]
        public void GivenIAmLoggedIn()
        {
            callback = new ClientCallback();
            instanceContext = new InstanceContext(callback);
            proxy = new ClientProxy(instanceContext, binding, ea);
            proxy.SignIn(username, "123");

            sh = 8;
            eh = 5;
            sm = 10;
            em = 10;
        }
        
        [When(@"I press add save changes button")]
        public void WhenIPressAddSaveChangesButton()
        {
            employee.Username = username;
            employee.StartHour = sh;
            employee.StartMinute = sm;
            employee.EndHour = eh;
            employee.EndMinute = em;
            proxy.SetWorkingHours(employee.Username,employee.StartHour,employee.StartMinute,employee.EndHour,employee.EndMinute);
        }
        
        [Then(@"my working hours should be changed")]
        public void ThenMyWorkingHoursShouldBeChanged()
        {
            Assert.AreEqual(sh,employee.StartHour);
            Assert.AreEqual(eh, employee.EndHour);
            Assert.AreEqual(sm, employee.StartMinute);
            Assert.AreEqual(em, employee.EndMinute);
        }
    }
}
