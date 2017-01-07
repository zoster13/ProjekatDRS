using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon;

namespace EmployeeCommonTest.DataTest
{
    [TestFixture]
    public class CurrentDataTest
    {
        private CurrentData currentDataTest;
        private List<Employee> employeesData = new List<Employee>();

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.currentDataTest = new CurrentData();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new CurrentData());
        }

        [Test]
        public void EmployeesDataTest()
        {
            currentDataTest.EmployeesData = employeesData;

            Assert.AreEqual(employeesData, currentDataTest.EmployeesData);
        }
    }
}
