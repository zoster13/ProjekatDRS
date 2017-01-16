using ClientCommon.Data;
using ICommon;
using NSubstitute;
using NUnit.Framework;
using Server;
using Server.Access;
using Server.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    [TestFixture]
    public class OutsourcingServiceTest
    {
        private OutsourcingService outsourcingServiceTest;

        private List<UserStoryCommon> commonUserStories;
        private UserStoryCommon usc1 = new UserStoryCommon() { Title = "usc1" };
        private UserStoryCommon usc2 = new UserStoryCommon() { Title = "usc2" };

        private string projectName = "proj1";

        private Project projectTest;
        private Team teamTest;
        private Employee employeeTestSM;


        [OneTimeSetUp]
        public void SetupTest()
        {
            outsourcingServiceTest = new OutsourcingService();
            EmployeeServiceDatabase.Instance = Substitute.For<IEmployeeServiceDatabase>();
            
            teamTest = new Team() { Name = "Team1", TeamLeaderEmail = "ivan@gmail.com" };

            employeeTestSM = new Employee(EmployeeType.SCRUMMASTER, "Ivan", "Markovic", "ivan@gmail.com", "ivan123", teamTest);
            
            projectTest = new Project() { Name = "proj1", Team = teamTest };

            commonUserStories = new List<UserStoryCommon>() { usc1, usc2 };
            
            //Mocking
            EmployeeServiceDatabase.Instance.UpdateUserStoriesStatus(commonUserStories, projectName).Returns(projectTest);
        }

        //AskForPartnership
        [Test]
        public void AskForPartnershipTest()
        {
            outsourcingServiceTest.AskForPartnership("hiringCompany13");
        }

        //SendEvaluatedUserstoriesToOutsourcingCompany
        [Test]
        public void SendEvaluatedUserstoriesToOutsourcingCompanyTest()
        {
            InternalDatabase.Instance.OnlineEmployees.Add(employeeTestSM);

            outsourcingServiceTest.SendEvaluatedUserstoriesToOutsourcingCompany(commonUserStories, projectName);
        }

        [Test]
        public void SendEvaluatedUserstoriesToOutsourcingCompanyTest2()
        {
            InternalDatabase.Instance.OnlineEmployees = new List<Employee>();

            outsourcingServiceTest.SendEvaluatedUserstoriesToOutsourcingCompany(commonUserStories, projectName);
        }


        //SendProjectToOutsourcingCompany
        [Test]
        public void SendProjectToOutsourcingCompanyTest()
        {
            outsourcingServiceTest.SendProjectToOutsourcingCompany("hiringCompany13", new ProjectCommon());
        }
    }
}
