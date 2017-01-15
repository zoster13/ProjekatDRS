using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using HiringCompany;
using EmployeeCommon;
using HiringCompany.Services;
using EmployeeCommon.Data;
using HiringCompany.DatabaseAccess;
using NSubstitute;
using System.ServiceModel;

namespace HiringCompanyTest.Services
{
    [TestFixture]
    public class EmployeeServiceTest
    {
        private EmployeeService employeeServiceTest;
        private Employee employeeTest;
        private Employee employeeTestSM;
        private Employee employeeTestNull;

        private Project projectTest;

        bool result;
        private InternalDatabase iDB;
        private PartnerCompany partnerCompanyTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            employeeServiceTest = new EmployeeService();
            HiringCompanyDB.Instance = Substitute.For<IHiringCompanyDB>();

            partnerCompanyTest = new PartnerCompany("prva");

            List<string> partnerCompanies = new List<string>();
            partnerCompanies.Add(partnerCompanyTest.Name);
            HiringCompanyDB.Instance.GetPartnerCompaniesNames().Returns(partnerCompanies);

            iDB = InternalDatabase.Instance();

            employeeTest = new Employee("zklasnic", "456", EmployeeType.CEO, "Zvezdana", "Klasnic", "zklasnic94@gmail.com", 9, 0, 17, 0);
            employeeTestSM = new Employee("rzekanovic", "112", EmployeeType.SM, "Radislav", "Zekanovic", "zklasnic94@gmail.com", 9, 20, 18, 40);
            employeeTestNull = null;

            projectTest = new Project("pokusaj", "nassnj", "zklasnic", "rzekanovic");

            iDB.OnlineEmployees.Add(employeeTest);
            IEmployeeServiceCallback callbackClient = Substitute.For<IEmployeeServiceCallback>();

            ICommunicationObject cObject = callbackClient as ICommunicationObject;
            iDB.ConnectionChannelsClients.Add("zklasnic",callbackClient);

                

            HiringCompanyDB.Instance.GetEmployee(Arg.Is<string>(username => username == "zklasnic")).Returns(employeeTest);
            HiringCompanyDB.Instance.GetEmployee(Arg.Is<string>(username => username == "rzekanovic")).Returns(employeeTestSM);
            HiringCompanyDB.Instance.GetEmployee(Arg.Is<string>(username=>(username != "rzekanovic" && username != "zklasnic"))).Returns(employeeTestNull);

            HiringCompanyDB.Instance.ClearEmployeeNotifs(Arg.Is<string>(username => username == "zklasnic")).Returns(true);
            HiringCompanyDB.Instance.ClearEmployeeNotifs(Arg.Is<string>(username => username == "rzekanovic")).Returns(true);


            HiringCompanyDB.Instance.AddNewEmployee(Arg.Is<Employee>(employeeTest)).Returns(true);
            HiringCompanyDB.Instance.AddNewEmployee(Arg.Is<Employee>(employeeTestSM)).Returns(true);
            HiringCompanyDB.Instance.AddNewEmployee(Arg.Is<Employee>(employeeTestNull)).Returns(false);

            HiringCompanyDB.Instance.AddNewProject(Arg.Is<Project>(projectTest)).Returns(true);

            HiringCompanyDB.Instance.EditEmployeeData("zklasnic", "Zvezdana", "Klasnic", "zklasnic94@gmail.com", "456").Returns(true);
            //iDB.EditOnlineEmployeeData("zklasnic", "Zvezdana", "Klasnic", "zklasnic94@gmail.com", "456").Returns(true);

            HiringCompanyDB.Instance.EditWorkingHours("zklasnic", 10, 20, 17, 30).Returns(true);
            //iDB.EditWorkingHoursForOnlineEm("zklasnic", 10, 20, 17, 30).Returns(true);

            HiringCompanyDB.Instance.EditEmployeeType("zklasnic", EmployeeType.CEO).Returns(true);
            //iDB.EditOnlineEmployeeType("zklasnic", EmployeeType.CEO).Returns(true);

            HiringCompanyDB.Instance.ProjectApprovedCEOFieldChange(projectTest).Returns(true);
            HiringCompanyDB.Instance.CloseProjectFieldChange(projectTest.Name).Returns(true);
        }

        [Test]
        public void AddNewEmployeeTestOk()
        {
            result = employeeServiceTest.AddNewEmployee(employeeTest);
            Assert.IsTrue(result);
        }

        [Test]
        public void AddNewEmployeeTestFault()
        {
            result= employeeServiceTest.AddNewEmployee(new Employee() { Username = "jjovanovic" });
            Assert.IsFalse(result);
        }

        [Test] // videcu da li radi
        public void SignInTestOk()
        {
            result = employeeServiceTest.SignIn("zklasnic", "456");
            Assert.IsTrue(result);
        }

        [Test]
        public void SignInTestFaultUsername()
        {
            result = employeeServiceTest.SignIn("jjovanovic", "123");
            Assert.IsFalse(result);
        }

        [Test]
        public void SignInTestFaultPassword()
        {
            result = employeeServiceTest.SignIn("zklasnic", "123");
            Assert.IsFalse(result);
        }

        [Test]
        public void SignOutTestOk() //ako ne radi kako treba,to je jer treba gore u testu da dodam u onlineEmployees onog kojeg hocu da signOut
        {
            Assert.DoesNotThrow(() => { employeeServiceTest.SignOut(employeeTest.Username); });
        }

        [Test]
        public void ChangeEmployeeDataTest() //trebalo bi da radi
        {
            Assert.DoesNotThrow(() => { employeeServiceTest.ChangeEmployeeData("zklasnic", "Zvezdana", "Klasnic", "zklasnic94@gmail.com", "456"); });
        }

        [Test]
        public void SetWorkingHoursTest()
        {
            Assert.DoesNotThrow(() => { employeeServiceTest.SetWorkingHours("zklasnic", 10, 20, 17, 30); });
        }

        [Test]
        public void ChangeEmployeeTypeTest()
        {
            Assert.DoesNotThrow(() => { employeeServiceTest.ChangeEmployeeType("zklasnic", EmployeeType.CEO); });
        }

        [Test]
        public void CreateNewProjectTest()
        {
            Assert.DoesNotThrow(() => { employeeServiceTest.CreateNewProject(projectTest); });
        }

        [Test]
        public void ProjectApprovedByCeo()
        {
            Assert.DoesNotThrow(() => { employeeServiceTest.ProjectApprovedByCeo(projectTest); });
        }

        [Test]
        public void CloseProjectTest()
        {
            Assert.DoesNotThrow(() => { employeeServiceTest.CloseProject(projectTest.Name); });
        }
    }
}

