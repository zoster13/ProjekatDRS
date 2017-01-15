using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCommonTest.DataTest
{
    [TestFixture]
    public class NotificationTest
    {
        private Notification notificationTest;
        private NotificationType type = NotificationType.PROJECT_REQUEST;
        private string hiringCompanyName = "kompanija";
        private string projectName = "projekat";
        private int id = 1;
        private string projectDescription = "nesto o projektu";
        private DateTime someTime = DateTime.Now;
        private string message = "poruka";
        private Employee employee = new Employee(EmployeeType.DEVELOPER, "mare", "maric", "mare", "mare", null);

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.notificationTest = new Notification();
        }

        #region Tests

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new Notification());
        }

        [Test]
        public void ConstructorTestWithParametersTypePartnership()
        {
            Notification n1 = new Notification(NotificationType.REQUEST_FOR_PARTNERSHIP, "kompanija", "projekat", "opis");
        }

        [Test]
        public void ConstructorTestWithParametersTypeProject()
        {
            Assert.DoesNotThrow(() => new Notification(NotificationType.PROJECT_REQUEST, "kompanija", "projekat", "opis"));
        }

        [Test]
        public void ConstructorTestWithParametersType()
        {
            Assert.DoesNotThrow(() => new Notification(NotificationType.NEW_PROJECT_TL, "kompanija", "projekat", "opis"));
        }

        [Test]
        public void IdTest()
        {
            notificationTest.Id = id;

            Assert.AreEqual(id, notificationTest.Id);
        }

        [Test]
        public void TypeTest()
        {
            notificationTest.Type = type;

            Assert.AreEqual(type, notificationTest.Type);
        }

        [Test]
        public void NotificationStampTest()
        {
            notificationTest.NotificationStamp = someTime;

            Assert.AreEqual(notificationTest.NotificationStamp, someTime);
        }

        [Test]
        public void NotificationAcceptStatusTest()
        {
            notificationTest.StatusAccept = NotificationAcceptStatus.ACCEPTED;

            Assert.AreEqual(notificationTest.StatusAccept, NotificationAcceptStatus.ACCEPTED);
        }

        [Test]
        public void NotificationNewStatusTest()
        {
            notificationTest.StatusNew = NotificationNewStatus.NEW;

            Assert.AreEqual(notificationTest.StatusNew, NotificationNewStatus.NEW);
        }

        [Test]
        public void MessageTest()
        {
            notificationTest.Message = message;

            Assert.AreEqual(notificationTest.Message, message);
        }

        [Test]
        public void HiringCompanyNameTest()
        {
            notificationTest.HiringCompanyName = hiringCompanyName;

            Assert.AreEqual(notificationTest.HiringCompanyName, hiringCompanyName);
        }

        [Test]
        public void ProjectCompanyNameTest()
        {
            notificationTest.ProjectName = projectName;

            Assert.AreEqual(notificationTest.ProjectName, projectName);
        }

        [Test]
        public void ProjectDescriptionTest()
        {
            notificationTest.ProjectDescription = projectDescription;

            Assert.AreEqual(notificationTest.ProjectDescription, projectDescription);
        }

        [Test]
        public void EmployeeTest()
        {
            notificationTest.Emoloyee = employee;

            Assert.AreEqual(notificationTest.Emoloyee, employee);
        }

        #endregion Tests
    }
}
