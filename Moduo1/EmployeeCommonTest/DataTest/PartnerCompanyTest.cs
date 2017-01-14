using EmployeeCommon;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon.Data;

namespace EmployeeCommonTest.DataTest
{
    [TestFixture]
    public class PartnerCompanyTest
    {
        private PartnerCompany partnerCompanyTest;
        private string name = "DMS";

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.partnerCompanyTest = new PartnerCompany();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new PartnerCompany());
        }

        [Test]
        public void ConstructorTestWithParameters()
        {
            Assert.DoesNotThrow(() => new PartnerCompany(name));

            partnerCompanyTest = new PartnerCompany(name);

            Assert.AreEqual(name, partnerCompanyTest.Name);
        }

        [Test]
        public void NameTest()
        {
            partnerCompanyTest.Name = name;

            Assert.AreEqual(name, partnerCompanyTest.Name);
        }
    }
}
