using EmployeeCommon;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommonTest.DataTest
{
    [TestFixture]
    public class NotificationTest
    {
        private Notification notificationTest;
        private EmployeeType destination = EmployeeType.CEO;
        private NotificationType notifType = NotificationType.ONLINE_EMPLOYEES;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.notificationTest = new Notification();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new Notification());
        }

        [Test]
        public void DestionationTest()
        {
            notificationTest.Destination = destination;

            Assert.AreEqual(destination, notificationTest.Destination);
        }

        [Test]
        public void NotifTypeTest()
        {
            notificationTest.NotifType = notifType;

            Assert.AreEqual(notifType, notificationTest.NotifType);
        }
    }
}
