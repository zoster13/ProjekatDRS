using EmployeeCommon;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommonTest.DataTest
{
    [TestFixture]
    public class ProjectTest
    {
        private Project projectTest;
        private int id = 1;
        private string name = "ProjectOne";
        private string description = "blabla";
        private DateTime startDate = System.DateTime.Now;
        private DateTime deadline = System.DateTime.Now;
        private List<UserStory> userStories = new List<UserStory>();
        private string outsourcingCompany = "DMS";
        private string productOwner = "zklasnic";

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.projectTest = new Project();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new Project());
        }

        [Test]
        public void IdTest()
        {
            projectTest.Id = id;

            Assert.AreEqual(id, projectTest.Id);
        }

        [Test]
        public void NameTest()
        {
            projectTest.Name = name;

            Assert.AreEqual(name, projectTest.Name);
        }

        [Test]
        public void DescriptionTest()
        {
            projectTest.Description = description;

            Assert.AreEqual(description, projectTest.Description);
        }

        [Test]
        public void StartDateTest()
        {
            projectTest.StartDate = startDate;

            Assert.AreEqual(startDate, projectTest.StartDate);
        }

        [Test]
        public void DeadlineTest()
        {
            projectTest.Deadline = deadline;

            Assert.AreEqual(deadline, projectTest.Deadline);
        }

        [Test]
        public void UserStoriesTest()
        {
            projectTest.UserStories = userStories;

            Assert.AreEqual(userStories, projectTest.UserStories);
        }

        [Test]
        public void OutsourcingCompanyTest()
        {
            projectTest.OutsourcingCompany = outsourcingCompany;

            Assert.AreEqual(outsourcingCompany, projectTest.OutsourcingCompany);
        }

        [Test]
        public void ProductOwnerTest()
        {
            projectTest.ProductOwner = productOwner;

            Assert.AreEqual(productOwner, projectTest.ProductOwner);
        }
    }
}
