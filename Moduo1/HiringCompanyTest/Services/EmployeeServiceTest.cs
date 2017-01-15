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

        [OneTimeSetUp]
        public void SetupTest()
        {
            employeeServiceTest = new EmployeeService();
            HiringCompanyDB.Instance = Substitute.For<IHiringCompanyDB>();
            iDB = InternalDatabase.Instance();

            
            //instance = Substitute.For<IHiringCompanyDB>();

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

            HiringCompanyDB.Instance.AddNewEmployee(Arg.Is<Employee>(employeeTest)).Returns(true);
            HiringCompanyDB.Instance.AddNewEmployee(Arg.Is<Employee>(employeeTestSM)).Returns(true);
            HiringCompanyDB.Instance.AddNewEmployee(Arg.Is<Employee>(employeeTestNull)).Returns(false);

            HiringCompanyDB.Instance.AddNewProject(Arg.Is<Project>(projectTest)).Returns(true);

            HiringCompanyDB.Instance.EditEmployeeData("zklasnic", "Zvezdana", "Klasnic", "zklasnic94@gmail.com", "456").Returns(true);
            HiringCompanyDB.Instance.EditWorkingHours("zklasnic", 10, 20, 17, 30).Returns(true);
            HiringCompanyDB.Instance.EditEmployeeType("zklasnic", EmployeeType.CEO).Returns(true);
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

        //[Test] // valjda ne radi zbog IEmployeeServiceCallback callbackClient = OperationContext.Current.GetCallbackChannel<IEmployeeServiceCallback>();
        //public void SignInTestOk()
        //{
        //    result = employeeServiceTest.SignIn("zklasnic", "456");
        //    Assert.IsTrue(result);
        //}

        //[Test]
        //public void SignInTestAlreadyExist()
        //{
        //    result = employeeServiceTest.SignIn("zklasnic", "456");
        //    Assert.IsFalse(result);
        //}

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
        public void SignOutTestOk()
        {
            Assert.DoesNotThrow(() => { employeeServiceTest.SignOut(employeeTest.Username); });
        }

        [Test]
        public void ChangeEmployeeDataTest()
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

