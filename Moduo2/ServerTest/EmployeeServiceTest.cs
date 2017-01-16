using ClientCommon.Data;
using ClientCommon.TempStructure;
using ICommon;
using NSubstitute;
using NUnit.Framework;
using Server;
using Server.Access;
using Server.Database;
using System;
using System.Collections.Generic;

namespace ServerTest
{
    [TestFixture]
    public class EmployeeServiceTest
    {
        private EmployeeService employeeServiceTest;

        private Employee employeeTest;
        private Employee employeeTestSM;
        private Employee employeeTestNull;
        private Team teamTest;
        private Team teamTestNull;
        private UserStory userStoryTest;
        private Task taskTest;
        private Task taskTestNull;
        private Task taskTest2;
        private Project projectTest;
        private string projectName1;
        private string projectName2;
        private List<UserStoryCommon> commonUserStories;


        [OneTimeSetUp]
        public void SetupTest()
        {
            employeeServiceTest = new EmployeeService();
            EmployeeServiceDatabase.Instance = Substitute.For<IEmployeeServiceDatabase>();

            teamTest = new Team() { Name = "Team1", TeamLeaderEmail = "marko@gmail.com" };
            teamTestNull = null;

            employeeTest = new Employee(EmployeeType.DEVELOPER, "Marko", "Markovic", "marko@gmail.com", "mare123", teamTest);
            employeeTest.WorkingHoursStart = DateTime.Now.AddHours(2);
            employeeTest.PasswordTimeStamp = DateTime.Now;

            TimeSpan timeSpan20Min = new TimeSpan(0, 20, 0);

            employeeTestSM = new Employee(EmployeeType.SCRUMMASTER, "Ivan", "Markovic", "ivan@gmail.com", "ivan123", teamTest);
            employeeTestSM.PasswordTimeStamp = DateTime.Now - timeSpan20Min;
            employeeTestSM.WorkingHoursStart = DateTime.Now - timeSpan20Min;

            employeeTestNull = null;

            taskTest = new Task() { Title = "task1", ProgressStatus = ProgressStatus.COMPLETED };
            taskTest2 = new Task() { Title = "task2", ProgressStatus = ProgressStatus.STARTED };
            taskTestNull = null;

            userStoryTest = new UserStory() { Title = "us1", Tasks = new List<Task>() { taskTest, taskTest2 } };

            projectTest = new Project() { Name = "proj1", Team = teamTest };
            projectName1 = "proj1";
            projectName2 = "proj2";

            commonUserStories = new List<UserStoryCommon>();

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
            EmployeeServiceDatabase.Instance.AddTask(Arg.Is<Task>(taskTest2)).Returns(false);
            
            EmployeeServiceDatabase.Instance.ReleaseUserStory(Arg.Is<UserStory>(userStoryTest)).Returns(userStoryTest);

            EmployeeServiceDatabase.Instance.TaskClaimed(Arg.Is<Task>(taskTest)).Returns(taskTest);

            EmployeeServiceDatabase.Instance.TaskCompleted(Arg.Is<Task>(taskTest)).Returns(new TaskAndUserStoryCompletedFlag() { Task = taskTest, UserStoryCompletedFlag = true });

            EmployeeServiceDatabase.Instance.UpdateProjectStatus(Arg.Is<string>("proj1")).Returns(true);
            EmployeeServiceDatabase.Instance.UpdateProjectStatus(Arg.Is<string>("proj2")).Returns(false);

            EmployeeServiceDatabase.Instance.GetAllEmployees().Returns(new List<Employee>() { employeeTest, employeeTestSM });

            EmployeeServiceDatabase.Instance.UpdateUserStoriesStatus(commonUserStories, projectName1).Returns(projectTest);

            EmployeeServiceDatabase.Instance.GetAllUserStories().Returns(new List<UserStory>() { userStoryTest });
        }

        //LogIn Tests
        [Test]
        public void LogInTestOk()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.LogIn("marko@gmail.com", "mare123"));
        }

        [Test]
        public void LogInTestEmployeeAlreadyExist()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.LogIn("marko@gmail.com", "mare123"));
        }

        [Test]
        public void LogInTestSM()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.LogIn("ivan@gmail.com", "ivan123"));
        }

        [Test]
        public void LogInTestFaultEmail()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.LogIn("sara@gmail.com", "sara123"));
        }

        [Test]
        public void LogInTestFaultPass()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.LogIn("marko@gmail.com", "sara123"));
        }

        //LogOut Tests
        [Test]
        public void LogOutTestOk()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.LogOut(employeeTest));
        }

        //AddEmployee Tests
        [Test]
        public void AddEmployeeTestOk()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddEmployee(employeeTest));
        }

        [Test]
        public void AddEmployeeTestOkSM()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddEmployee(employeeTestSM));
        }
        
        [Test]
        public void AddEmployeeTestFault()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddEmployee(new Employee() { Email = "sanja@gmai.com" }));
        }

        //EditEmployee Tests
        [Test]
        public void EditEmployeeTestOk()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.EditEmployeeData(employeeTest));
        }

        [Test]
        public void EditEmployeeTestFault()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.EditEmployeeData(employeeTestSM));
        }
        
        //AddTeam Tests
        [Test]
        public void AddTeamTestOk()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddTeam(teamTest));
        }

        [Test]
        public void AddTeamTestFault()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddTeam(teamTestNull));
        }

        //AddTeamAndTL Tests
        [Test]
        public void AddTeamAndTLTestOk()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddTeamAndTL(teamTest, employeeTest));
        }

        [Test]
        public void AddTeamAndTLTestFaultTeamNull()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddTeamAndTL(teamTestNull, employeeTest));
        }

        [Test]
        public void AddTeamAndTLTestFaultEmployeeNull()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddTeamAndTL(teamTest, employeeTestNull));
        }

        //AddTeamAndUpdateDeveloperToTL
        [Test]
        public void AddTeamAndUpdateDeveloperToTLTestOk()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddTeamAndUpdateDeveloperToTL(teamTest, employeeTest));
        }

        [Test]
        public void AddTeamAndUpdateDeveloperToTLTestFaultTeamNull()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddTeamAndUpdateDeveloperToTL(teamTestNull, employeeTest));
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
            Assert.DoesNotThrow(() => employeeServiceTest.ProjectTeamAssign(new Project() { Name = "proj1", Team = teamTest }));
        }

        //AddUserStory
        [Test]
        public void AddUserStoryTestOk()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddUserStory(userStoryTest, projectName1));
        }

        [Test]
        public void AddUserStoryTestFault()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddUserStory(userStoryTest, projectName2));
        }

        //AddTask
        [Test]
        public void AddTaskTestOk()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddTask(taskTest));
        }

        [Test]
        public void AddTaskTestFault()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.AddTask(taskTestNull));
        }

        //ReleaseUserStory
        [Test]
        public void ReleaseUserStoryTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.ReleaseUserStory(userStoryTest));
        }

        //TaskClaimed
        [Test]
        public void TaskClaimedTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.TaskClaimed(taskTest));
        }

        //TaskCompleted
        [Test]
        public void TaskCompletedTest()
        {
            Assert.Catch(() => employeeServiceTest.TaskCompleted(taskTest));
        }

        //SendUserStories
        [Test]
        public void SendUserStoriesTestOk()
        {
            Assert.Catch(() => employeeServiceTest.SendUserStories(commonUserStories, projectName1));
        }

        [Test]
        public void SendUserStoriesTestFault()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.SendUserStories(commonUserStories, projectName2));
        }


        //ResponseToPartnershipRequest
        [Test]
        public void ResponseToPartnershipRequestTestAccepted()
        {
            Assert.Catch(() => employeeServiceTest.ResponseToPartnershipRequest(true, "hiringCompany1"));
        }

        [Test]
        public void ResponseToPartnershipRequestTestDeclined()
        {
            Assert.Catch(() => employeeServiceTest.ResponseToPartnershipRequest(false, "hiringCompany1"));
        }

        //ResponseToProjectRequest
        [Test]
        public void ResponseToProjectRequestTestAccepted()
        {
            Assert.Catch(() => employeeServiceTest.ResponseToProjectRequest(true, new Project()));
        }

        [Test]
        public void ResponseToProjectRequestTestDeclined()
        {
            Assert.Catch(() => employeeServiceTest.ResponseToProjectRequest(false, new Project()));
        }

        //GetUserStories
        [Test]
        public void GetUserStoriesTest()
        {
            Assert.DoesNotThrow(() => Assert.DoesNotThrow(() => employeeServiceTest.GetUserStories()));
        }

        //GetAllTasks
        [Test]
        public void GetAllTasksTest()
        {
            Assert.DoesNotThrow(() => Assert.DoesNotThrow(() => employeeServiceTest.GetAllTasks()));
        }

        //NotifyOnLate
        [Test]
        public void NotifyOnLateTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.NotifyOnLate(null, null));
        }

        //PasswordExpired
        [Test]
        public void PasswordExpiredTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.PasswordExpired(null, null));
        }

        //UserStoryCompleted
        [Test]
        public void UserStoryCompletedTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.UserStoryCompleted(null, null));
        }

        //NotifySMForUserStoryProgress
        [Test]
        public void NotifySMForUserStoryProgressTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.NotifySMForUserStoryProgress(employeeTestSM.Email, userStoryTest.Title));
        }

        [Test]
        public void NotifySMForUserStoryProgressTest2()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.NotifySMForUserStoryProgress("sinan@gmail.com", userStoryTest.Title));
        }

    }
}