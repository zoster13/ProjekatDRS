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
        Team teamTest;
        Team teamTestNull;
        
        [OneTimeSetUp]
        public void SetupTest()
        {
            employeeServiceTest = new EmployeeService();
            EmployeeServiceDatabase.Instance = Substitute.For<IEmployeeServiceDatabase>();

            employeeTest = new Employee(EmployeeType.DEVELOPER, "Marko", "Markovic", "marko@gmail.com", "mare123", new Team());
            employeeTestSM = new Employee(EmployeeType.SCRUMMASTER, "Ivan", "Markovic", "ivan@gmail.com", "ivan123", new Team());
            emloyeeTestNull = null;
            teamTest = new Team() { Name = "Team1" };
            teamTestNull = null;

            //Mocking
            EmployeeServiceDatabase.Instance.GetEmployee(Arg.Is<string>(email => email == "marko@gmail.com")).Returns(employeeTest);
            EmployeeServiceDatabase.Instance.GetEmployee(Arg.Is<string>(email => email == "ivan@gmail.com")).Returns(employeeTestSM);
            EmployeeServiceDatabase.Instance.GetEmployee(Arg.Is<string>(email => (email != "ivan@gmail.com" && email != "marko@gmail.com"))).Returns(emloyeeTestNull);
            
            EmployeeServiceDatabase.Instance.AddEmployee(Arg.Is<Employee>(employeeTest)).Returns(true);
            EmployeeServiceDatabase.Instance.AddEmployee(Arg.Is<Employee>(employeeTestSM)).Returns(true);
            EmployeeServiceDatabase.Instance.AddEmployee(Arg.Is<Employee>(emloyeeTestNull)).Returns(false);

            EmployeeServiceDatabase.Instance.UpdateEmployee(Arg.Is<Employee>(employeeTest)).Returns(true);
            EmployeeServiceDatabase.Instance.UpdateEmployee(Arg.Is<Employee>(employeeTestSM)).Returns(false);

            EmployeeServiceDatabase.Instance.AddTeam(Arg.Is<Team>(teamTest)).Returns(true);
            EmployeeServiceDatabase.Instance.AddTeam(Arg.Is<Team>(teamTestNull)).Returns(false);
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


        //EditEmployee Tests
        [Test]
        public void EditEmployeeTestOk()
        {
            employeeServiceTest.EditEmployeeData(employeeTest);
        }

        [Test]
        public void EditEmployeeTestFault()
        {
            employeeServiceTest.EditEmployeeData(employeeTestSM);
        }

        
        //AddTeam Tests
        [Test]
        public void AddTeamTestOk()
        {
            employeeServiceTest.AddTeam(teamTest);
        }

        [Test]
        public void AddTeamTestFault()
        {
            employeeServiceTest.AddTeam(teamTestNull);
        }

        //AddTeamAndTL Tests
        [Test]
        public void AddTeamAndTLTestOk()
        {
            employeeServiceTest.AddTeamAndTL(teamTest, employeeTest);
        }

        [Test]
        public void AddTeamAndTLTestFaultTeamNull()
        {
            employeeServiceTest.AddTeamAndTL(teamTestNull, employeeTest);
        }

        [Test]
        public void AddTeamAndTLTestFaultEmployeeNull()
        {
            employeeServiceTest.AddTeamAndTL(teamTest, emloyeeTestNull);
        }
    }
}
