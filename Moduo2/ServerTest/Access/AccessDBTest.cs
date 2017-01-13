using NUnit.Framework;
using Server.Access;
using System.Linq;

namespace ServerTest.Access
{
    [TestFixture]
    public class AccessDBTest
    {
        private AccessDB accessTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.accessTest = new AccessDB();
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