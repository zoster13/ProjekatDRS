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
    public class ProjectCommonTest
    {
        private ProjectCommon projectCommonTest;
        private string name = "ProjectOne";
        private string description = "blabla";
        private DateTime startDate = System.DateTime.Now;
        private DateTime deadline = System.DateTime.Now;
        private bool isAcceptedByOutsCompany = false;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.projectCommonTest = new ProjectCommon();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new ProjectCommon());
        }

        [Test]
        public void ConstructorTestWithParameters()
        {
            Assert.DoesNotThrow(() => new ProjectCommon(name, description, startDate, deadline));

            projectCommonTest = new ProjectCommon(name, description, startDate, deadline);

            Assert.AreEqual(name, projectCommonTest.Name);
            Assert.AreEqual(description, projectCommonTest.Description);
            Assert.AreEqual(startDate, projectCommonTest.StartDate);
            Assert.AreEqual(deadline, projectCommonTest.Deadline);
            Assert.AreEqual(isAcceptedByOutsCompany, projectCommonTest.IsAcceptedByOutsCompany);
        }

        [Test]
        public void NameTest()
        {
            projectCommonTest.Name = name;

            Assert.AreEqual(name, projectCommonTest.Name);
        }

        [Test]
        public void DescriptionTest()
        {
            projectCommonTest.Description = description;

            Assert.AreEqual(description, projectCommonTest.Description);
        }

        [Test]
        public void StartDateTest()
        {
            projectCommonTest.StartDate = startDate;

            Assert.AreEqual(startDate, projectCommonTest.StartDate);
        }

        [Test]
        public void DeadlineTest()
        {
            projectCommonTest.Deadline = deadline;

            Assert.AreEqual(deadline, projectCommonTest.Deadline);
        }

        [Test]
        public void IsAcceptedByOutsCompanyTest()
        {
            projectCommonTest.IsAcceptedByOutsCompany = isAcceptedByOutsCompany;

            Assert.AreEqual(isAcceptedByOutsCompany, projectCommonTest.IsAcceptedByOutsCompany);
        }
    }
}
