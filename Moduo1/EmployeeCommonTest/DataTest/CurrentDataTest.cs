using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon;
using EmployeeCommon.Data;

namespace EmployeeCommonTest.DataTest
{
    [TestFixture]
    public class CurrentDataTest
    {
        private CurrentData currentDataTest;
        private List<Employee> employeesData = new List<Employee>();
        private List<Employee> allEmployeesData = new List<Employee>();
        private List<Project> projectsForApprovalData = new List<Project>();
        private List<Project> projectsForSendingData = new List<Project>();
        private List<Project> projectsInDevelopmentData = new List<Project>();
        private List<String> namesOfCompaniesData = new List<string>();
        private List<PartnerCompany> companiesData = new List<PartnerCompany>();

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.currentDataTest = new CurrentData();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new CurrentData());
        }

        [Test]
        public void EmployeesDataTest()
        {
            currentDataTest.EmployeesData = employeesData;

            Assert.AreEqual(employeesData, currentDataTest.EmployeesData);
        }

        [Test]
        public void AllEmployeesDataTest()
        {
            currentDataTest.AllEmployeesData = allEmployeesData;

            Assert.AreEqual(allEmployeesData, currentDataTest.AllEmployeesData);
        }

        [Test]
        public void ProjectsForApprovalDataTest()
        {
            currentDataTest.ProjectsForApprovalData = projectsForApprovalData;

            Assert.AreEqual(projectsForApprovalData, currentDataTest.ProjectsForApprovalData);
        }

        [Test]
        public void ProjectsForSendingDataTest()
        {
            currentDataTest.ProjectsForSendingData = projectsForSendingData;

            Assert.AreEqual(projectsForSendingData, currentDataTest.ProjectsForSendingData);
        }

        [Test]
        public void ProjectsInDevelopmentDataTest()
        {
            currentDataTest.ProjectsInDevelopmentData = projectsInDevelopmentData;

            Assert.AreEqual(projectsInDevelopmentData, currentDataTest.ProjectsInDevelopmentData);
        }

        [Test]
        public void NamesOfCompaniesDataTest()
        {
            currentDataTest.NamesOfCompaniesData = namesOfCompaniesData;

            Assert.AreEqual(namesOfCompaniesData, currentDataTest.NamesOfCompaniesData);
        }

        [Test]
        public void CompaniesDataTest()
        {
            currentDataTest.CompaniesData = companiesData;

            Assert.AreEqual(companiesData, currentDataTest.CompaniesData);
        }
    }
}
