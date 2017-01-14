using ClientCommon.Data;
using NSubstitute;
using NUnit.Framework;
using Server;
using Server.Access;

namespace ServerTest
{
    [TestFixture]
    public class EmployeeServiceTest
    {
        private EmployeeService employeeServiceTest;

        Employee employeeTest;
        Employee employeeTestSM;
        Employee emloyeeTestNull;
        
        
        [OneTimeSetUp]
        public void SetupTest()
        {
            employeeServiceTest = new EmployeeService();
            EmployeeServiceDatabase.Instance = Substitute.For<IEmployeeServiceDatabase>();

            employeeTest = new Employee(EmployeeType.DEVELOPER, "Marko", "Markovic", "marko@gmail.com", "mare123", new Team());
            employeeTestSM = new Employee(EmployeeType.SCRUMMASTER, "Ivan", "Markovic", "ivan@gmail.com", "ivan123", new Team());
            emloyeeTestNull = null;

            //Mocking
            EmployeeServiceDatabase.Instance.GetEmployee(Arg.Is<string>(email => email == "marko@gmail.com")).Returns(employeeTest);
            EmployeeServiceDatabase.Instance.GetEmployee(Arg.Is<string>(email => email == "ivan@gmail.com")).Returns(employeeTestSM);
            EmployeeServiceDatabase.Instance.GetEmployee(Arg.Is<string>(email => (email != "ivan@gmail.com" && email != "marko@gmail.com"))).Returns(emloyeeTestNull);


            EmployeeServiceDatabase.Instance.AddEmployee(Arg.Is<Employee>(employeeTest)).Returns(true);

        }
        
        //LogIn Tests
        [Test]
        public void LogInTestOk()
        {
            employeeServiceTest.LogIn("marko@gmail.com", "mare123");
        }

        [Test]
        public void LogInTestEmployeeAlreadyExist()
        {
            employeeServiceTest.LogIn("marko@gmail.com", "mare123");
        }

        [Test]
        public void LogInTestSM()
        {
            employeeServiceTest.LogIn("ivan@gmail.com", "ivan123");
        }

        [Test]
        public void LogInTestFaultEmail()
        {
            employeeServiceTest.LogIn("sara@gmail.com", "sara123");
        }

        [Test]
        public void LogInTestFaultPass()
        {
            employeeServiceTest.LogIn("marko@gmail.com", "sara123");
        }

        //LogOut Tests
        [Test]
        public void LogOutTestOk()
        {
            employeeServiceTest.LogOut(employeeTest);
        }


        //AddEmployee Tests
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
