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
        public void ConstructorTestWithParameters()
        {
            //Assert.DoesNotThrow(() => new Notification(type, hiringCompanyName, projectName));

            Assert.AreEqual(type, notificationTest.Type);
            Assert.AreEqual(hiringCompanyName, notificationTest.HiringCompanyName);
            Assert.AreEqual(projectName, notificationTest.ProjectName);
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

        //[Test]
        //public void NameTest()
        //{
        //    employeeTest.Name = name;

        //    Assert.AreEqual(name, employeeTest.Name);
        //}

        //[Test]
        //public void SurnameTest()
        //{
        //    employeeTest.Surname = surname;

        //    Assert.AreEqual(surname, employeeTest.Surname);
        //}

        //[Test]
        //public void EmailTest()
        //{
        //    employeeTest.Email = email;

        //    Assert.AreEqual(email, employeeTest.Email);
        //}

        //[Test]
        //public void PasswordTest()
        //{
        //    employeeTest.Password = password;

        //    Assert.AreEqual(password, employeeTest.Password);
        //}

        #endregion Tests
    }
}
