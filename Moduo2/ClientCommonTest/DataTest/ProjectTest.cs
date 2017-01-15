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
    public class ProjectTest
    {
        private Project projectTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.projectTest = new Project();
        }

        [Test]
        [TestCase("projekat", "kompanija")]
        public void ProjectConstructorWithParametersTest(string projName, string hiringCompanyName)
        {
            Assert.DoesNotThrow(() => new Project(projName, hiringCompanyName));
        }

        [Test]
        [TestCase(2)]
        public void IdTest(int id)
        {
            projectTest.Id = id;

            Assert.AreEqual(id, projectTest.Id);
        }

        [Test]
        [TestCase("projekat")]
        public void NameTest(string name)
        {
            projectTest.Name = name;

            Assert.AreEqual(name, projectTest.Name);
        }

        [Test]
        [TestCase("desc")]
        public void DescriptionTest(string desc)
        {
            projectTest.Description = desc;

            Assert.AreEqual(desc, projectTest.Description);
        }

        [Test]
        [TestCase("kompanija")]
        public void HiringCompanyNameTest(string kompanija)
        {
            projectTest.HiringCompanyName = kompanija;

            Assert.AreEqual(kompanija, projectTest.HiringCompanyName);
        }

        [Test]
        public void AssignStatusTest()
        {
            projectTest.AssignStatus = AssignStatus.ASSIGNED;

            Assert.AreEqual(projectTest.AssignStatus, AssignStatus.ASSIGNED);
        }

        [Test]
        public void ProgressStatusTest()
        {
            projectTest.ProgressStatus = ProgressStatus.COMPLETED;

            Assert.AreEqual(projectTest.ProgressStatus, ProgressStatus.COMPLETED);
        }

        [Test]
        public void UserStoriesTest()
        {
            List<UserStory> usl = new List<UserStory>();
            projectTest.UserStories = usl;

            Assert.AreEqual(projectTest.UserStories, usl);
        }

        [Test]
        public void TeamTeam()
        {
            Team team =  new Team();
            projectTest.Team = team;

            Assert.AreEqual(projectTest.Team, team);
        }

        [Test]
        public void ToStringTest()
        {
            projectTest.Name = "projekat";

            Assert.AreEqual(projectTest.ToString(), "projekat");
        }
    }
}
