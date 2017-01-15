using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;
using ClientCommon.Data;
using System.Threading;

namespace ClientTest
{
    [TestFixture]
    public class CallbackMethodsTest
    {
        private CallbackMethods callbackMethods;
        private static Employee employeeCEOTest;

        [OneTimeSetUp]
        public void CallbackMethodsTestt()
        {
            Thread t = new Thread(() => callbackMethods = new CallbackMethods());
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            employeeCEOTest = new Employee() { Name = "marko", Surname = "markovic", Type = EmployeeType.CEO, Email = "marko@gmail.com", Password = "mare123" };
        }

        [Test]
        public void LogInCallbackTrueTest()
        {
            Assert.DoesNotThrow(() => callbackMethods.mainWindow.LogInCallbackResult(employeeCEOTest, true));
        }

        //[Test]
        //public void LogInCallbackFalseTest()
        //{
        //    Assert.DoesNotThrow(() => callbackMethods.LogInCallback(employeeCEOTest, false));
        //}

        //[Test]
        //public void EditEmployeeCallbackTest()
        //{
        //    Assert.DoesNotThrow(() => callbackMethods.EditEmployeeCallback(employeeCEOTest));
        //}
    }
}
