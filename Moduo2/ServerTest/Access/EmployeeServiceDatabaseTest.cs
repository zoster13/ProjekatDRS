using ClientCommon.Data;
using NSubstitute;
using NUnit.Framework;
using Server.Access;

namespace ServerTest.Access
{
    [TestFixture]
    public class EmployeeServiceDatabaseTest
    {
        private IEmployeeServiceDatabase dbTest;
        private bool result;
        

        [OneTimeSetUp]
        public void SetupTest()
        {
            dbTest = Substitute.For<IEmployeeServiceDatabase>();

            dbTest.AddEmployee(Arg.Is<Employee>(e => e.Email == "marko@gmail.com")).Returns(true);
            dbTest.AddEmployee(Arg.Is<Employee>(e => e.Email != "marko@gmail.com")).Returns(false);
        }

        [Test]
        public void AddEmployeeTestOk()
        {
            Employee employee = new Employee() { Email = "marko@gmail.com" };
            result = dbTest.AddEmployee(employee);

            Assert.IsTrue(result);
        }

        [Test]
        public void AddEmployeeTestFault()
        {
            Employee employee = new Employee() { Email = "ivan@gmail.com" };
            result = dbTest.AddEmployee(employee);

            Assert.IsFalse(result);
        }
    }
}
