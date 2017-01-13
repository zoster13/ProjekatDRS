using ClientCommon.Data;
using NSubstitute;
using NUnit.Framework;
using Server.Access;

namespace ServerTest.Access
{
    [TestFixture]
    public class EmployeeServiceDatabaseTest
    {
        private IEmployeeServiceDatabase dbInstanceMockTest;
        private IEmployeeServiceDatabase dbInstanceTest;
        bool result;
        
        [OneTimeSetUp]
        public void SetupTest()
        {
            dbInstanceTest = EmployeeServiceDatabase.Instance;
        }

        [Test]
        public void AddEmployeeTest()
        {
            Assert.DoesNotThrow(() => dbInstanceTest.AddEmployee(new Employee() { Email = "marko@gmail.com" }));
            Assert.AreEqual(true, dbInstanceTest.AddEmployee(new Employee() { Email = "marko@gmail.com" }));
        }

    }
}
