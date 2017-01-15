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
    public class HiringCompanyTest
    {
        private HiringCompany hiringCompanyTest;
        private string name = "DMS";
        private int id = 2;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.hiringCompanyTest = new HiringCompany();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new Employee());
        }

        [Test]
        public void ConstructorTestWithParameters()
        {

            Assert.DoesNotThrow(() => new HiringCompany(name));
            hiringCompanyTest = new HiringCompany(name);

            Assert.AreEqual(name, hiringCompanyTest.Name);

        }

        [Test]
        public void IdTest()
        {
            hiringCompanyTest.Id = id;

            Assert.AreEqual(id, hiringCompanyTest.Id);
        }

        [Test]
        public void NameTest()
        {
            hiringCompanyTest.Name = name;

            Assert.AreEqual(name, hiringCompanyTest.Name);
        }

        [Test]
        public void ToStringTest()
        {
            hiringCompanyTest.Name = "kompanija";

            Assert.AreEqual(hiringCompanyTest.ToString(), "kompanija");
        }

    }
}