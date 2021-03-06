﻿using Client;
using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class EditEmployeeDataSteps
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        private EmployeeProxy proxy;

        private Employee employee;

        [Given(@"I a form for editing data")]
        public void GivenIAFormForEditingData()
        {
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());

            employee = new Employee();
        }
        
        [When(@"I change ""(.*)"", ""(.*)"", ""(.*)"",")]
        public void WhenIChange(string p0, string p1, string p2)
        {
            employee.Name = p0;
            employee.Surname = p1;
            employee.Password = p2;
            employee.Email = "nenad@gmail.com";
        }
        
        [Then(@"the changes are displayed")]
        public void ThenTheChangesAreDisplayed()
        {
            Assert.DoesNotThrow(() => proxy.EditEmployeeData(employee));
        }
    }
}
