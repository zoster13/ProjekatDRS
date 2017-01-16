using ClientCommon.Data;
using NUnit.Framework;
using Server.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest.Database
{
    [TestFixture]
    public class InternalDatabaseTest
    {
        private InternalDatabase instanceTest;
        private Employee employeeTest;
        private Employee employeeTest2;
        private Employee employeeTestNull;


        [OneTimeSetUp]
        public void SetupTest()
        {
            this.instanceTest = InternalDatabase.Instance;
            instanceTest.OnlineEmployees = new List<Employee>();
            employeeTest = new Employee(EmployeeType.DEVELOPER, "Marko", "Markovic", "mare@gmail.com", "mare123", new Team());
            employeeTest2 = new Employee(EmployeeType.DEVELOPER, "Markic", "Marko", "mare@gmail.com", "123", new Team());
            employeeTestNull = new Employee(EmployeeType.DEVELOPER, "Markic", "Marko", "marko@gmail.com", "123", new Team());
        }

        [Test]
        public void OnlineEmployeesTest()
        {
            Assert.DoesNotThrow(() => instanceTest.OnlineEmployees.Add(employeeTest));
            Assert.DoesNotThrow(() => instanceTest.OnlineEmployees.FirstOrDefault());
        }

        [Test]
        public void LockerOnlineEmployeesTest()
        {
            object obj = new object();
            instanceTest.LockerOnlineEmployees = obj;

            Assert.AreEqual(obj, instanceTest.LockerOnlineEmployees);
        }

        [Test]
        public void UpdateOnlineEmployeeTest()
        {
            Assert.DoesNotThrow(() => instanceTest.UpdateOnlineEmployee(employeeTest2));
            Assert.DoesNotThrow(() => instanceTest.UpdateOnlineEmployee(employeeTestNull));
        }

        [Test]
        public void UpdateDeveloperToTLTest()
        {
            Assert.DoesNotThrow(() => instanceTest.UpdateDeveloperToTL(employeeTest2, new Team() { Name = "Novi tim" }));
            Assert.DoesNotThrow(() => instanceTest.UpdateDeveloperToTL(employeeTestNull, new Team() { Name = "Novi tim" }));
        }

    }
}
