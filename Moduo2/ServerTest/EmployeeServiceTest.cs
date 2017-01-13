using ClientCommon.Data;
using NSubstitute;
using NUnit.Framework;
using Server;
using Server.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    [TestFixture]
    public class EmployeeServiceTest
    {
        private EmployeeService employeeServiceTest;

        Employee employeeTest;
        Employee employeeTestSM;


        [OneTimeSetUp]
        public void SetupTest()
        {
            employeeServiceTest = new EmployeeService();
            EmployeeServiceDatabase.Instance = Substitute.For<IEmployeeServiceDatabase>();

            employeeTest = new Employee(EmployeeType.DEVELOPER, "Marko", "Markovic", "marko@gmail.com", "mare123", new Team());
            employeeTestSM = new Employee(EmployeeType.SCRUMMASTER, "Ivan", "Markovic", "ivan@gmail.com", "ivan123", new Team());

            //Mocking
            EmployeeServiceDatabase.Instance.GetEmployee(Arg.Is<string>(email => email == "marko@gmail.com")).Returns(employeeTest);
            EmployeeServiceDatabase.Instance.GetEmployee(Arg.Is<string>(email => email == "ivan@gmail.com")).Returns(employeeTestSM);

            EmployeeServiceDatabase.Instance.AddEmployee(Arg.Is<Employee>(employeeTest)).Returns(true);

        }
        
        [Test]
        public void LogInTestOk()
        {
            employeeServiceTest.LogIn("marko@gmail.com", "mare123");
        }

        [Test]
        public void LogInTestExist()
        {
            employeeServiceTest.LogIn("marko@gmail.com", "mare123");
        }

        [Test]
        public void LogInTestFault1()
        {
            employeeServiceTest.LogIn("ivan@gmail.com", "ivan123");
        }

        [Test]
        public void LogInTestFault2()
        {
            employeeServiceTest.LogIn("marko@gmail.com", "ivan123");
        }

        [Test]
        public void LogOutTestOk()
        {
            employeeServiceTest.LogOut(employeeTest);
        }

        [Test]
        public void AddEmployeeTestOk()
        {
            employeeServiceTest.AddEmployee(employeeTest);
        }

        [Test]
        public void AddEmployeeTestOkSM()
        {
            employeeServiceTest.AddEmployee(employeeTestSM);
        }

        [Test]
        public void AddEmployeeTestFault()
        {
            employeeServiceTest.AddEmployee(new Employee() { Email = "sanja@gmai.com" });
        }

    }
}
