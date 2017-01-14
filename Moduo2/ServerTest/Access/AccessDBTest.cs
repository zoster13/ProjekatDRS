using ClientCommon.Data;
using NSubstitute;
using NUnit.Framework;
using Server.Access;
using System.Linq;

namespace ServerTest.Access
{
    [TestFixture]
    public class AccessDBTest
    {
        private AccessDB accessTest;
        private Employee employeeTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.accessTest = new AccessDB();
            employeeTest = new Employee(EmployeeType.DEVELOPER, "Marko", "Markovic", "marko@gmail.com", "mare123", new Team());
        }

        [Test]
        public void EmployeeTest()
        {
            Assert.DoesNotThrow(() => accessTest.Employees.FirstOrDefault());
        }

        [Test]
        public void TeamsTest()
        {
            Assert.DoesNotThrow(() => accessTest.Teams.FirstOrDefault());
        }

        [Test]
        public void NotificationsTest()
        {
            Assert.DoesNotThrow(() => accessTest.Notifications.FirstOrDefault());
        }

        [Test]
        public void ProjectsTest()
        {
            Assert.DoesNotThrow(() => accessTest.Projects.FirstOrDefault());
        }

        [Test]
        public void HiringCompaniesTest()
        {
            Assert.DoesNotThrow(() => accessTest.HiringCompanies.FirstOrDefault());
        }

        [Test]
        public void UserStoriesTest()
        {
            Assert.DoesNotThrow(() => accessTest.UserStories.FirstOrDefault());
        }

        [Test]
        public void TasksTest()
        {
            Assert.DoesNotThrow(() => accessTest.Tasks.FirstOrDefault());
        }
    }
}