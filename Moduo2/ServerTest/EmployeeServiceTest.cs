using ClientCommon.Data;
using ClientCommon.TempStructure;
using ICommon;
using NSubstitute;
using NUnit.Framework;
using Server;
using Server.Access;
using Server.Database;
using System.Collections.Generic;

namespace ServerTest
{
    [TestFixture]
    public class EmployeeServiceTest
    {
        private EmployeeService employeeServiceTest;

        Employee employeeTest;
        Employee employeeTestSM;
        Employee employeeTestNull;
        Team teamTest;
        Team teamTestNull;
        UserStory userStoryTest;
        Task taskTest;
        Task taskTestNull;


        [OneTimeSetUp]
        public void SetupTest()
        {
            employeeServiceTest = new EmployeeService();
            EmployeeServiceDatabase.Instance = Substitute.For<IEmployeeServiceDatabase>();

            employeeTest = new Employee(EmployeeType.DEVELOPER, "Marko", "Markovic", "marko@gmail.com", "mare123", new Team());
            employeeTestSM = new Employee(EmployeeType.SCRUMMASTER, "Ivan", "Markovic", "ivan@gmail.com", "ivan123", new Team());
            employeeTestNull = null;
            teamTest = new Team() { Name = "Team1", TeamLeaderEmail=employeeTest.Email };
            teamTestNull = null;
            userStoryTest = new UserStory() { Title = "us1" };
            taskTest = new Task() { Title = "task1" };
            taskTestNull = null;

            InternalDatabase.Instance.OnlineEmployees.Add(employeeTest);
            
            //Mocking
            EmployeeServiceDatabase.Instance.GetEmployee(Arg.Is<string>(email => email == "marko@gmail.com")).Returns(employeeTest);
            EmployeeServiceDatabase.Instance.GetEmployee(Arg.Is<string>(email => email == "ivan@gmail.com")).Returns(employeeTestSM);
            EmployeeServiceDatabase.Instance.GetEmployee(Arg.Is<string>(email => (email != "ivan@gmail.com" && email != "marko@gmail.com"))).Returns(employeeTestNull);
            
            EmployeeServiceDatabase.Instance.AddEmployee(Arg.Is<Employee>(employeeTest)).Returns(true);
            EmployeeServiceDatabase.Instance.AddEmployee(Arg.Is<Employee>(employeeTestSM)).Returns(true);
            EmployeeServiceDatabase.Instance.AddEmployee(Arg.Is<Employee>(employeeTestNull)).Returns(false);

            EmployeeServiceDatabase.Instance.UpdateEmployee(Arg.Is<Employee>(employeeTest)).Returns(true);
            EmployeeServiceDatabase.Instance.UpdateEmployee(Arg.Is<Employee>(employeeTestSM)).Returns(false);

            EmployeeServiceDatabase.Instance.AddTeam(Arg.Is<Team>(teamTest)).Returns(true);
            EmployeeServiceDatabase.Instance.AddTeam(Arg.Is<Team>(teamTestNull)).Returns(false);

            EmployeeServiceDatabase.Instance.UpdateEmployeeFunctionAndTeam(employeeTest, teamTest.Name).Returns(true);
            EmployeeServiceDatabase.Instance.UpdateEmployeeFunctionAndTeam(employeeTestNull, "team1").Returns(false);
            EmployeeServiceDatabase.Instance.UpdateEmployeeFunctionAndTeam(employeeTest, "team2").Returns(false);

            EmployeeServiceDatabase.Instance.UpdateProjectsTeam("proj1", teamTest.Name).Returns(teamTest);

            EmployeeServiceDatabase.Instance.AddUserStory(userStoryTest, "proj1").Returns(true);
            EmployeeServiceDatabase.Instance.AddUserStory(userStoryTest, "proj2").Returns(false);

            EmployeeServiceDatabase.Instance.AddTask(Arg.Is<Task>(taskTest)).Returns(true);
            EmployeeServiceDatabase.Instance.AddTask(Arg.Is<Task>(taskTestNull)).Returns(false);

            EmployeeServiceDatabase.Instance.ReleaseUserStory(Arg.Is<UserStory>(userStoryTest)).Returns(userStoryTest);

            EmployeeServiceDatabase.Instance.TaskCompleted(Arg.Is<Task>(taskTest)).Returns(new TaskAndUserStoryCompletedFlag() { Task = taskTest, UserStoryCompletedFlag = true });

            EmployeeServiceDatabase.Instance.UpdateProjectStatus(Arg.Is<string>("proj1")).Returns(true);
            EmployeeServiceDatabase.Instance.UpdateProjectStatus(Arg.Is<string>("proj2")).Returns(false);
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
            employeeServiceTest.AddTeamAndTL(teamTest, employeeTestNull);
        }

        //AddTeamAndUpdateDeveloperToTL
        [Test]
        public void AddTeamAndUpdateDeveloperToTLTestOk()
        {
            employeeServiceTest.AddTeamAndUpdateDeveloperToTL(teamTest, employeeTest);
        }

        [Test]
        public void AddTeamAndUpdateDeveloperToTLTestFaultTeamNull()
        {
            employeeServiceTest.AddTeamAndUpdateDeveloperToTL(teamTestNull, employeeTest);
        }

        [Test]
        public void AddTeamAndUpdateDeveloperToTLTestFaultEmployeeNull()
        {
            employeeServiceTest.AddTeamAndUpdateDeveloperToTL(teamTest, employeeTestNull);
        }

        //GetAllOnlineEmployees
        [Test]
        public void GetAllOnlineEmployeesTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.GetAllOnlineEmployees());
        }

        //GetAllEmployees
        [Test]
        public void GetAllEmployeesTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.GetAllEmployees());
        }

        //GetAllTeams
        [Test]
        public void GetAllTeamsTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.GetAllTeams());
        }

        //GetAllHiringCompanies
        [Test]
        public void GetAllHiringCompaniesTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.GetAllHiringCompanies());
        }

        //GetAllProjects
        [Test]
        public void GetAllProjectsTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.GetAllProjects());
        }

        //ProjectTeamAssign
        [Test]
        public void ProjectTeamAssignTestTest()
        {
            employeeServiceTest.ProjectTeamAssign(new Project() { Name = "proj1", Team = teamTest });
        }

        //AddUserStory
        [Test]
        public void AddUserStoryTestOk()
        {
            employeeServiceTest.AddUserStory(userStoryTest, "proj1");
        }

        [Test]
        public void AddUserStoryTestFault()
        {
            employeeServiceTest.AddUserStory(userStoryTest, "proj2");
        }

        //AddTask
        [Test]
        public void AddTaskTestOk()
        {
            employeeServiceTest.AddTask(taskTest);
        }

        [Test]
        public void AddTaskTestFault()
        {
            employeeServiceTest.AddTask(taskTestNull);
        }

        //ReleaseUserStory
        [Test]
        public void ReleaseUserStoryTest()
        {
            employeeServiceTest.ReleaseUserStory(userStoryTest);
        }

        //TaskClaimed
        [Test]
        public void TaskClaimedTest()
        {
            employeeServiceTest.TaskClaimed(taskTest);
        }

        //TaskCompleted
        [Test]
        public void TaskCompletedTest()
        {
            employeeServiceTest.TaskCompleted(taskTest);
        }

        //SendUserStories
        [Test]
        public void SendUserStoriesTestOk()
        {
            employeeServiceTest.SendUserStories(new List<UserStoryCommon>(), "proj1");
        }

        [Test]
        public void SendUserStoriesTestFault()
        {
            employeeServiceTest.SendUserStories(new List<UserStoryCommon>(), "proj2");
        }

    }
}
