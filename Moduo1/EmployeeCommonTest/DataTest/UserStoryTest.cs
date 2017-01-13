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

    }
}
