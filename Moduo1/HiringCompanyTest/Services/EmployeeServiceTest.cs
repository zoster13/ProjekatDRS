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

namespace HiringCompanyTest.Services
{
    [TestFixture]
    public class EmployeeServiceTest
    {
        private EmployeeService employeeServiceTest;
        private Employee employeeTest;
        private Employee employeeTestSM;
        private Employee employeeTestNull;
        bool result;

        [OneTimeSetUp]
        public void SetupTest()
        {
            employeeServiceTest = new EmployeeService();
            HiringCompanyDB.Instance = Substitute.For<IHiringCompanyDB>();

            //instance = Substitute.For<IHiringCompanyDB>();

            employeeTest = new Employee("zklasnic", "456", EmployeeType.CEO, "Zvezdana", "Klasnic", "zklasnic94@gmail.com", 9, 0, 17, 0);
            employeeTestSM = new Employee("rzekanovic", "112", EmployeeType.SM, "Radislav", "Zekanovic", "zklasnic94@gmail.com", 9, 20, 18, 40);
            employeeTestNull = null;

            HiringCompanyDB.Instance.GetEmployee(Arg.Is<string>(username => username == "zklasnic")).Returns(employeeTest);
            HiringCompanyDB.Instance.GetEmployee(Arg.Is<string>(username => username == "rzekanovic")).Returns(employeeTestSM);
            HiringCompanyDB.Instance.GetEmployee(Arg.Is<string>(username=>(username != "rzekanovic" && username != "zklasnic"))).Returns(employeeTestNull);

            HiringCompanyDB.Instance.AddNewEmployee(Arg.Is<Employee>(employeeTest)).Returns(true);
        }

        [Test]
        public void AddNewEmployeeTestOk()
        {
            result= employeeServiceTest.AddNewEmployee(employeeTest);
            Assert.IsTrue(result);
        }

        [Test]
        public void AddNewEmployeeTestFault()
        {
            result= employeeServiceTest.AddNewEmployee(new Employee() { Username = "jjovanovic" });
            Assert.IsFalse(result);
        }

        //[Test]
        //public void SignInTestOk()
        //{
        //    result= employeeServiceTest.SignIn("zklasnic", "456");
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

        //[Test]
        //public void SignOutTestOk()
        //{
        //    Assert.DoesNotThrow(()=> { employeeServiceTest.SignOut(employeeTest.Username); }) ;
        //}       

    }
}

