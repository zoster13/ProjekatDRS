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
        private DateTime workTimeEmployee;

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

            TimeSpan timeSpan20Min = new TimeSpan(0, 20, 0);
            DateTime dt = DateTime.Now - timeSpan20Min;

            employeeTest = new Employee("zklasnic", "456", EmployeeType.CEO, "Zvezdana", "Klasnic", "zklasnic94@gmail.com", 9, 0, 17, 0);
            employeeTest.DatePasswordChanged = DateTime.Parse("10/05/2016");
            //employeeTest.StartHour = dt.Hour;
            //employeeTest.StartMinute = dt.Minute;
            //employeeTest.StartHour = dt.Hour;
            //employeeTest.StartMinute = dt.Minute;

            employeeTestSM = new Employee("rzekanovic", "112", EmployeeType.SM, "Radislav", "Zekanovic", "zklasnic94@gmail.com", 9, 20, 18, 40);
            employeeTestSM.DatePasswordChanged = DateTime.Now - timeSpan20Min;
            employeeTestSM.StartHour = dt.Hour;
            employeeTestSM.StartMinute = dt.Minute;

            employeeTestNull = null;

            projectTest = new Project("pokusaj", "nassnj", "zklasnic", "rzekanovic");
            TimeSpan timeSpan9Days = new TimeSpan(9, 0, 0, 0);
            projectTest.Deadline = DateTime.Now + timeSpan9Days;
            
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

            List<Employee> allEmployees = new List<Employee>();
            allEmployees.Add(employeeTestSM);
            allEmployees.Add(employeeTest);

            Employee em = new Employee("pperic", "112", EmployeeType.HR, "Pera", "Peric", "pperic@gmail.com", 9, 0, 17, 0);
            em.DatePasswordChanged = DateTime.Now;
            em.StartHour = DateTime.Now.Hour;
            em.StartMinute = DateTime.Now.Minute;
            allEmployees.Add(em);

            HiringCompanyDB.Instance.AllEmployees().Returns(allEmployees);

            Project pr = new Project("blabla", "ndsusbij", "amisic", "zklasnic");
            UserStory us1 = new UserStory("nsdusdnu", "sdjis", "sdjidsni");
            pr.UserStories.Add(us1);
            pr.IsAcceptedCEO = true;
            pr.IsAcceptedOutsCompany = true;
            pr.Deadline = DateTime.Now + timeSpan9Days;

            Project pr2 = new Project("blabla", "ndsusbij", "amisic", "zklasnic");
            UserStory us2 = new UserStory("nsdusdnu", "sdjis", "sdjidsni");
            pr2.UserStories.Add(us2);
            pr2.IsAcceptedCEO = true;
            pr2.IsAcceptedOutsCompany = true;
            pr2.Deadline = DateTime.Now.AddMonths(1);

            Project pr3 = new Project("blabla", "ndsusbij", "amisic", "zklasnic");
            UserStory us3 = new UserStory("nsdusdnu", "sdjis", "sdjidsni");
            UserStory us4 = new UserStory("nsdusdnu", "sdjis", "sdjidsni");
            us3.IsClosed = true;
            us4.IsClosed = true;
            pr3.UserStories.Add(us3);
            pr3.UserStories.Add(us4);
            pr3.IsAcceptedCEO = true;
            pr3.IsAcceptedOutsCompany = true;
            pr3.Deadline = DateTime.Now + timeSpan9Days;

            Project pr4 = new Project("sndjvnds", "nsdjbjs", "amisic", "zklasnic");
            pr4.IsClosed = true;

            Project pr5 = new Project("ksdlmdk", "andjan", "amisic", "zklasnic");
            pr5.IsAcceptedCEO = true;
            pr5.IsAcceptedOutsCompany = true;

            Project pr6 = new Project("nadijnsij", "sdidbsi", "amisic", "zklasnic");
            pr6.UserStories.Add(us2);

            Project pr7 = new Project("nadijnsij", "sdidbsi", "amisic", "zklasnic");
            pr7.UserStories.Add(us2);
            pr7.IsAcceptedCEO = true;

            List<Project> projs = new List<Project>();
            projs.Add(projectTest);
            projs.Add(pr);
            projs.Add(pr2);
            projs.Add(pr3);
            projs.Add(pr4);
            projs.Add(pr5);
            projs.Add(pr6);
            projs.Add(pr7);
            HiringCompanyDB.Instance.ProjectsInDevelopment().Returns(projs);

            iDB.OnlineEmployees.Add(employeeTest);
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
        public void SignOutTestOk()
        {
            Assert.DoesNotThrow(() => { employeeServiceTest.SignOut(employeeTest.Username); });
        }

        [Test]
        public void SignOutTestFault()
        {
            Assert.DoesNotThrow(() => { employeeServiceTest.SignOut(employeeTestSM.Username); });
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

        //NotifyOnLate
        [Test]
        public void NotifyOnLateTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.NotifyOnLate(null, null));
        }

        //PasswordExpired
        [Test]
        public void NotifyPasswordExpiredTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.NotifyPasswordExpired(null, null));
        }

        //UserStoryCompleted
        [Test]
        public void NotifyUserStoriesDeadlineTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest.NotifyUserStoriesDeadline(null, null));
        }
    }
}

