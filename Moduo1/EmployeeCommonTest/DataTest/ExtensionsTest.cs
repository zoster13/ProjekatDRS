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
    public class ExtensionsTest
    {
        [Test]
        [TestCase(EmployeeType.CEO, "Chief Executive Officer")]
        [TestCase(EmployeeType.HR, "Human Resources")]
        [TestCase(EmployeeType.PO, "Product Owner")]
        [TestCase(EmployeeType.SM, "Scrum Master")]
        public void TypeToStringTestOk(EmployeeType type, string pos)
        {
            string position = Extensions.TypeToString(type);
            Assert.AreEqual(position, pos);
        }

        [Test]
        [TestCase(EmployeeType.CEO, "Chief Executive Officer")]
        [TestCase(EmployeeType.HR, "Human Resources")]
        [TestCase(EmployeeType.PO, "Product Owner")]
        [TestCase(EmployeeType.SM, "Scrum Master")]
        [TestCase(EmployeeType.CEO, "bla")]
        public void StringToType(EmployeeType type, string pos)
        {
            EmployeeType t = Extensions.StringToType(pos);
            Assert.AreEqual(t, type);
        }
    }
}
