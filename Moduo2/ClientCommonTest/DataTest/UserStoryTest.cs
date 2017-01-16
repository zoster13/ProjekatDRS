using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCommonTest.DataTest
{
    [TestFixture]
    public class UserStoryTest
    {
        private UserStory userStoryTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.userStoryTest = new UserStory();
        }

        [Test]
        public void IdTest()
        {
            userStoryTest.Id = 0;

            Assert.AreEqual(userStoryTest.Id, 0);
        }

        [Test]
        public void TitleTest()
        {
            userStoryTest.Title = "naslov";

            Assert.AreEqual(userStoryTest.Title, "naslov");
        }

        [Test]
        public void DescriptionTest()
        {
            userStoryTest.Description = "opis";

            Assert.AreEqual(userStoryTest.Description, "opis");
        }

        [Test]
        public void DifficultyTest()
        {
            userStoryTest.Difficulty = 2;

            Assert.AreEqual(userStoryTest.Difficulty, 2);
        }

        [Test]
        public void AcceptanceCriteriaTest()
        {
            userStoryTest.AcceptanceCriteria = "acc crit";

            Assert.AreEqual(userStoryTest.AcceptanceCriteria, "acc crit");
        }

        [Test]
        public void AcceptStatusTest()
        {
            userStoryTest.AcceptStatus = AcceptStatus.ACCEPTED;

            Assert.AreEqual(userStoryTest.AcceptStatus, AcceptStatus.ACCEPTED);
        }

        [Test]
        public void ProgressStatusTest()
        {
            userStoryTest.ProgressStatus = ProgressStatus.COMPLETED;

            Assert.AreEqual(userStoryTest.ProgressStatus, ProgressStatus.COMPLETED);
        }

        [Test]
        public void TasksTest()
        {
            List<ClientCommon.Data.Task> tasks = new List<ClientCommon.Data.Task>();
            userStoryTest.Tasks = tasks;

            Assert.AreEqual(userStoryTest.Tasks, tasks);
        }

        [Test]
        public void DeadlineTest()
        {
            DateTime deadline = new DateTime();
            userStoryTest.Deadline = deadline;

            Assert.AreEqual(userStoryTest.Deadline, deadline);
        }

        [Test]
        public void ProjectTest()
        {
            Project proj = new Project();
            userStoryTest.Project = proj;

            Assert.AreEqual(userStoryTest.Project, proj);
        }

        [Test]
        public void ToStringTest()
        {
            userStoryTest.Title = "us1";

            Assert.AreEqual(userStoryTest.ToString(), "us1");
        }
    }
}
