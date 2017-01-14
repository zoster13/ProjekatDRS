﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiringCompany.DatabaseAccess;
using EmployeeCommon.Data;
using NUnit.Framework;
using HiringCompany;
using EmployeeCommon;

namespace HiringCompanyTest.DatabaseAccessTest
{
    [TestFixture]
    public class InternalDatabaseTest
    {
        private InternalDatabase instanceTest;
        private Employee employeeTest;
        private Employee employeeTest2;
        private Employee employeeTestNull;
        private string companyName = "HiringCompany";

        private Dictionary<string, string> possiblePartnersAddresses = new Dictionary<string, string>();
        private Dictionary<string, string> partnerCompaniesAddresses = new Dictionary<string, string>();
        private Dictionary<string, OutsorcingCompProxy> connectionChannelsCompanies = new Dictionary<string, OutsorcingCompProxy>();
        private Dictionary<string, IEmployeeServiceCallback> connectionChannelsClients = new Dictionary<string, IEmployeeServiceCallback>();

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.instanceTest = InternalDatabase.Instance();
            instanceTest.OnlineEmployees = new List<Employee>();
            employeeTest = new Employee("jjovanovic", "123", EmployeeType.CEO, "Jovan", "Jovanovic", "jovan@gmail.com", 10, 20, 17, 30);
            employeeTest2 = new Employee("jjovanovic", "123", EmployeeType.CEO, "Jovan", "Jovanovic", "jovan@gmail.com", 10, 20, 17, 30);
            employeeTestNull = new Employee("jjovanovic", "123", EmployeeType.CEO, "Jovan", "Jovanovic", "jovan@gmail.com", 10, 20, 17, 30);
        }

        [Test]
        public void OnlineEmployeesTest()
        {
            Assert.DoesNotThrow(() => instanceTest.OnlineEmployees.Add(employeeTest));
            Assert.DoesNotThrow(() => instanceTest.OnlineEmployees.FirstOrDefault());
        }

        [Test]
        public void OnlineEmployees_lockTest()
        {
            object obj = new object();
            instanceTest.OnlineEmployees_lock = obj;

            Assert.AreEqual(obj, instanceTest.OnlineEmployees_lock);
        }

        [Test]
        public void ProjectsForApproval_lockTest()
        {
            object obj = new object();
            instanceTest.ProjectsForApproval_lock = obj;

            Assert.AreEqual(obj, instanceTest.ProjectsForApproval_lock);
        }

        [Test]
        public void PartnerCompaniesAddresses_lockTest()
        {
            object obj = new object();
            instanceTest.PartnerCompaniesAddresses_lock = obj;

            Assert.AreEqual(obj, instanceTest.PartnerCompaniesAddresses_lock);
        }

        [Test]
        public void ChannelsCompanies_lockTest()
        {
            object obj = new object();
            instanceTest.ChannelsCompanies_lock = obj;

            Assert.AreEqual(obj, instanceTest.ChannelsCompanies_lock);
        }

        [Test]
        public void ChannelsClients_lockTest()
        {
            object obj = new object();
            instanceTest.ChannelsClients_lock = obj;

            Assert.AreEqual(obj, instanceTest.ChannelsClients_lock);
        }

        [Test]
        public void PossiblePartnersAddressesTest()
        {
            instanceTest.PossiblePartnersAddresses = possiblePartnersAddresses;
            Assert.AreEqual(possiblePartnersAddresses, instanceTest.PossiblePartnersAddresses);
        }

        [Test]
        public void PartnerCompaniesAddressesTest()
        {
            instanceTest.PartnerCompaniesAddresses = partnerCompaniesAddresses;
            Assert.AreEqual(partnerCompaniesAddresses, instanceTest.PartnerCompaniesAddresses);
        }

        [Test]
        public void ConnectionChannelsCompaniesTest()
        {
            instanceTest.ConnectionChannelsCompanies = connectionChannelsCompanies;
            Assert.AreEqual(connectionChannelsCompanies, instanceTest.ConnectionChannelsCompanies);
        }

        [Test]
        public void ConnectionChannelsClientsTest()
        {
            instanceTest.ConnectionChannelsClients = connectionChannelsClients;
            Assert.AreEqual(connectionChannelsClients, instanceTest.ConnectionChannelsClients);
        }

        [Test]
        public void CompanyNameTest()
        {
            instanceTest.CompanyName = companyName;
            Assert.AreEqual(companyName, instanceTest.CompanyName);
        }
    }
}