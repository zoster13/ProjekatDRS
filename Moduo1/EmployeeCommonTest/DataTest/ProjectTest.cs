using EmployeeCommon;
using EmployeeCommon.Data;
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
        private string scrumMaster = "rzekanovic";
        private bool isAcceptedCEO = false;
        private bool isAcceptedOutsCompany = false;
        private bool isClosed = false;

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
        public void ConstructorTestWithParameters()
        {
            Assert.DoesNotThrow(() => new Project(name, description, productOwner, scrumMaster));

            projectTest = new Project(name, description, productOwner, scrumMaster);

            Assert.AreEqual(name, projectTest.Name);
            Assert.AreEqual(description, projectTest.Description);
            Assert.AreEqual(productOwner, projectTest.ProductOwner);
            Assert.AreEqual(scrumMaster, projectTest.ScrumMaster);
            Assert.AreEqual(isAcceptedCEO, projectTest.IsAcceptedCEO);
            Assert.AreEqual(isAcceptedOutsCompany, projectTest.IsAcceptedOutsCompany);
            Assert.AreEqual(isClosed, projectTest.IsClosed);
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

        [Test]
        public void ScrumMasterTest()
        {
            projectTest.ScrumMaster = scrumMaster;

            Assert.AreEqual(scrumMaster, projectTest.ScrumMaster);
        }

        [Test]
        public void IsAcceptedCEOTest()
        {
            projectTest.IsAcceptedCEO = isAcceptedCEO;

            Assert.AreEqual(isAcceptedCEO, projectTest.IsAcceptedCEO);
        }

        [Test]
        public void IsAcceptedOutsCompanyTest()
        {
            projectTest.IsAcceptedOutsCompany = isAcceptedOutsCompany;

            Assert.AreEqual(isAcceptedOutsCompany, projectTest.IsAcceptedOutsCompany);
        }

        [Test]
        public void IsClosedTest()
        {
            projectTest.IsClosed = isClosed;

            Assert.AreEqual(isClosed, projectTest.IsClosed);
        }
    }
}
