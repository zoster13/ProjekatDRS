using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICommon;
using NUnit.Framework;

namespace ICommonTest
{
    [TestFixture]
    public class UserStoryCommonTest
    {
        private UserStoryCommon userStoryCommonTest;
        private string title = "UserStoryOne";
        private string description = "blabla";
        private string acceptanceCriteria = "blaa";
        private bool isAccepted = false;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.userStoryCommonTest = new UserStoryCommon();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new UserStoryCommon());
        }

        [Test]
        public void ConstructorTestWithParameters()
        {
            Assert.DoesNotThrow(() => new UserStoryCommon(title, description, acceptanceCriteria, isAccepted));
            userStoryCommonTest = new UserStoryCommon(title, description, acceptanceCriteria, isAccepted);

            Assert.AreEqual(title, userStoryCommonTest.Title);
            Assert.AreEqual(description, userStoryCommonTest.Description);
            Assert.AreEqual(acceptanceCriteria, userStoryCommonTest.AcceptanceCriteria);
            Assert.AreEqual(isAccepted, userStoryCommonTest.IsAccepted);
        }

        [Test]
        public void TitleTest()
        {
            userStoryCommonTest.Title = title;

            Assert.AreEqual(title, userStoryCommonTest.Title);
        }

        [Test]
        public void DescriptionTest()
        {
            userStoryCommonTest.Description = description;

            Assert.AreEqual(description, userStoryCommonTest.Description);
        }

        [Test]
        public void AcceptanceCriteriaTest()
        {
            userStoryCommonTest.AcceptanceCriteria = acceptanceCriteria;

            Assert.AreEqual(acceptanceCriteria, userStoryCommonTest.AcceptanceCriteria);
        }

        [Test]
        public void IsAcceptedTest()
        {
            userStoryCommonTest.IsAccepted = isAccepted;

            Assert.AreEqual(isAccepted, userStoryCommonTest.IsAccepted);
        }
    }
}
