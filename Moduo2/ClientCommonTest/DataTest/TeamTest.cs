using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCommonTest.DataTest
{
    public class TeamTest
    {
        private Team teamTest;
        private string name = "Network Viewer";
        private int id = 2;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.teamTest = new Team();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new Team());
        }

        [Test]
        public void ConstructorTestWithParameters()
        {

            Assert.DoesNotThrow(() => new Team(name));
            teamTest = new Team(name);

            Assert.AreEqual(name, teamTest.Name);

        }

        [Test]
        public void IdTest()
        {
            teamTest.Id = id;

            Assert.AreEqual(id, teamTest.Id);
        }

        [Test]
        public void NameTest()
        {
            teamTest.Name = name;

            Assert.AreEqual(name, teamTest.Name);
        }
    }
}
