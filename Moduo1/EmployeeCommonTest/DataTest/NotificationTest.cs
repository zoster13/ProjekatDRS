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
    public class NotificationTest
    {
        private Notification notificationTest;
        private int id = 1;
        private string content = "Project created";
        private string timestamp = "11/2/2012";

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
        public void ConstructorTestWithParameters()
        {
            Assert.DoesNotThrow(() => new Notification(timestamp, content));
            notificationTest = new Notification(timestamp, content);

            Assert.AreEqual(timestamp, notificationTest.Timestamp);
            Assert.AreEqual(content, notificationTest.Content);
        }

        [Test]
        public void IdTest()
        {
            notificationTest.Id = id;

            Assert.AreEqual(id, notificationTest.Id);
        }

        [Test]
        public void ContentTest()
        {
            notificationTest.Content = content;

            Assert.AreEqual(content, notificationTest.Content);
        }

        [Test]
        public void TimestampTest()
        {
            notificationTest.Timestamp = timestamp;

            Assert.AreEqual(timestamp, notificationTest.Timestamp);
        }
    }
}
