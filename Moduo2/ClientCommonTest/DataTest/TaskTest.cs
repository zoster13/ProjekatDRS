using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientCommon.Data;

namespace ClientCommonTest.DataTest
{
    [TestFixture]
    public class TaskTest
    {
        private ClientCommon.Data.Task taskTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.taskTest = new ClientCommon.Data.Task();
        }

        [Test]
        public void IdTest()
        {
            taskTest.Id = 0;

            Assert.AreEqual(taskTest.Id, 0);
        }

        [Test]
        public void AssignStatusTest()
        {
            taskTest.AssignStatus = AssignStatus.ASSIGNED;

            Assert.AreEqual(taskTest.AssignStatus, AssignStatus.ASSIGNED);
        }

        [Test]
        public void ProgressStatusTest()
        {
            taskTest.ProgressStatus = ProgressStatus.COMPLETED;

            Assert.AreEqual(taskTest.ProgressStatus, ProgressStatus.COMPLETED);
        }

        [Test]
        public void DescriptionTest()
        {
            taskTest.Description = "opis";

            Assert.AreEqual(taskTest.Description, "opis");
        }

        [Test]
        public void TitleTest()
        {
            taskTest.Title = "naslov";

            Assert.AreEqual(taskTest.Title, "naslov");
        }

        [Test]
        public void UserStoryTest()
        {
            UserStory us = new UserStory();
            taskTest.UserStory = us;

            Assert.AreEqual(taskTest.UserStory, us);
        }

        [Test]
        public void EmployeeNameTest()
        {
            taskTest.EmployeeName = "marko";

            Assert.AreEqual(taskTest.EmployeeName, "marko");
        }

        [Test]
        public void ToStringTest()
        {
            taskTest.Title = "task";

            Assert.AreEqual(taskTest.ToString(), "task");
        }
    }
}
