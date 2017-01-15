using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon;
using Client;
using System.Threading;
using EmployeeCommon.Data;

namespace ClientTest
{
    [TestFixture]
    public class EmployeeServiceCallbackTest
    {
        ClientCallback callbackMethods;
        private static Employee employeeForTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            Thread t = new Thread(() => callbackMethods = new ClientCallback());
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            employeeForTest = new Employee() {Username="ceca",Password="111",Type=EmployeeType.CEO,Email="ceca@mail.com",
                StartHour=8,StartMinute=1,EndHour=17,EndMinute=2};
            
        }

        [Test]
        public void NotifyTest()
        {
            Assert.DoesNotThrow(() => callbackMethods.ClientDB.Main.NotifyEmployee("proba test notif"));
        }

    }
}
