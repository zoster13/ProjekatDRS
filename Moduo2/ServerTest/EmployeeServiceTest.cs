using ClientCommon.Data;
using NSubstitute;
using NUnit.Framework;
using Server;
using Server.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    [TestFixture]
    public class EmployeeServiceTest
    {
        private EmployeeService employeeServiceTest;
        private Employee employeeTest = new Employee(EmployeeType.DEVELOPER, "Marko", "Markovic", "mare@gmail.com", "mare123", new Team());
            
        [OneTimeSetUp]
        public void SetupTest()
        {
            Assert.DoesNotThrow(() => employeeServiceTest = new EmployeeService());

            //EmployeeServiceDatabase.Instance = Substitute.For<IEmployeeServiceDatabase>();

            //EmployeeServiceDatabase.Instance.AddEmployee(Arg.Is<Employee>(x => x.Email == "mare@gmail.com").Returns(true));

        }

        
    }
}
