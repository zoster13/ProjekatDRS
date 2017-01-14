using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon;
using EmployeeCommon.Data;

namespace EmployeeCommonTest.DataTest
{
    [TestFixture]
    public class UserStoryTest
    {
        private UserStory userStoryTest;
        private int id = 1;
        private string title = "UserStoryOne";
        private string description = "blabla";
        private string acceptanceCriteria = "blaa";
        private bool isApprovedByPO = false;
        private bool isClosed = false;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.userStoryTest = new UserStory();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new UserStory());
        }

        [Test]
        public void ConstructorTestWithParameters()
        {
            Assert.DoesNotThrow(() => new UserStory(title, description, acceptanceCriteria));
            userStoryTest = new UserStory(title, description, acceptanceCriteria);

            Assert.AreEqual(title, userStoryTest.Title);
            Assert.AreEqual(description, userStoryTest.Description);
            Assert.AreEqual(acceptanceCriteria, userStoryTest.AcceptanceCriteria);
            Assert.AreEqual(isApprovedByPO, userStoryTest.IsApprovedByPO);
            Assert.AreEqual(isClosed, userStoryTest.IsClosed);
        }

        [Test]
        public void IdTest()
        {
            userStoryTest.Id = id;

            Assert.AreEqual(id, userStoryTest.Id);
        }

        [Test]
        public void TitleTest()
        {
            userStoryTest.Title = title;

            Assert.AreEqual(title, userStoryTest.Title);
        }

        [Test]
        public void DescriptionTest()
        {
            userStoryTest.Description = description;

            Assert.AreEqual(description, userStoryTest.Description);
        }

        [Test]
        public void AcceptanceCriteriaTest()
        {
            userStoryTest.AcceptanceCriteria = acceptanceCriteria;

            Assert.AreEqual(acceptanceCriteria, userStoryTest.AcceptanceCriteria);
        }

        [Test]
        public void IsApprovedByPOTest()
        {
            userStoryTest.IsApprovedByPO = isApprovedByPO;

            Assert.AreEqual(isApprovedByPO, userStoryTest.IsApprovedByPO);
        }

        [Test]
        public void IsClosedTest()
        {
            userStoryTest.IsClosed = isClosed;

            Assert.AreEqual(isClosed, userStoryTest.IsClosed);
        }
    }
}
