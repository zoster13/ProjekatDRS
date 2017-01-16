using System;
using System.Collections.Generic;
using System.ServiceModel;
using Client;
using EmployeeCommon;
using EmployeeCommon.Data;
using HiringCompany;
using HiringCompany.Services;
using TechTalk.SpecFlow;
using NUnit.Framework;
using ICommon;

namespace BDD
{
    [Binding]
    public class AddPartnershipCompanySteps
    {
        private static string svcAddress = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress ea = new EndpointAddress(new Uri(svcAddress));
        private NetTcpBinding binding = new NetTcpBinding();
        private ClientProxy proxy;
        IEmployeeServiceCallback callback;
        InstanceContext instanceContext;

        private string outsCompanyName;
     

        [Given(@"I have form for choosing company that request will be sent to")]
        public void GivenIHaveFormForChoosingCompanyThatRequestWillBeSentTo()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            IEmployeeServiceCallback callback = new ClientCallback();
            InstanceContext instanceContext = new InstanceContext(callback);
            proxy = new ClientProxy(instanceContext, binding, ea);

            
        }
        
        [When(@"I press button")]
        public void WhenIPressButton()
        {
            outsCompanyName = "cekic";          
        }
        
        [Then(@"the outsorcing company should be contacted with request")]
        public void ThenTheOutsorcingCompanyShouldBeContactedWithRequest()
        {
            Assert.DoesNotThrow(() => proxy.AskForPartnership(outsCompanyName));
        }
    }
}
