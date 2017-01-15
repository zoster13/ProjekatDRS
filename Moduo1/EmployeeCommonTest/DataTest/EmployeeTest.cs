using EmployeeCommon;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon.Data;

namespace EmployeeCommonTest.Data
{
    [TestFixture]
    public class EmployeeTest
    {
        private Employee employeeTest;
        private string username = "zklasnic";
        private string password = "456";
        private EmployeeType type = EmployeeType.CEO;
        private string name = "Zvezdana";
        private string surname = "Klasnic";
        private string email = "zvezdana@gmail.com";
        private int startHour = 10;
        private int startMinute = 20;
        private int endHour = 16;
        private int endMinute = 30;
        private List<Notification> notifications = new List<Notification>();
        private DateTime datePasswordChanged = DateTime.Now;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.employeeTest = new Employee();
        }

        #region Tests

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new Employee());
        }

        [Test]
        public void ConstructorTestWithParameters()
        {

            Assert.DoesNotThrow(() => new Employee(username, password, type, name, surname, email, startHour, startMinute, endHour, endMinute));
            employeeTest = new Employee(username, password, type, name, surname, email, startHour, startMinute, endHour, endMinute);

            Assert.AreEqual(username, employeeTest.Username);
            Assert.AreEqual(password, employeeTest.Password);
            Assert.AreEqual(type, employeeTest.Type);
            Assert.AreEqual(name, employeeTest.Name);
            Assert.AreEqual(surname, employeeTest.Surname);
            Assert.AreEqual(email, employeeTest.Email);
            Assert.AreEqual(startHour, employeeTest.StartHour);
            Assert.AreEqual(startMinute, employeeTest.StartMinute);
            Assert.AreEqual(endHour, employeeTest.EndHour);
            Assert.AreEqual(endMinute, employeeTest.EndMinute);
        }

        [Test]
        public void UsernameTest()
        {
            employeeTest.Username = username;

            Assert.AreEqual(username, employeeTest.Username);
        }

        [Test]
        public void PasswordTest()
        {
            employeeTest.Password = password;

            Assert.AreEqual(password, employeeTest.Password);
        }

        [Test]
        public void TypeTest()
        {
            employeeTest.Type = type;

            Assert.AreEqual(type, employeeTest.Type);
        }

        [Test]
        public void NameTest()
        {
            employeeTest.Name = name;

            Assert.AreEqual(name, employeeTest.Name);
        }

        [Test]
        public void SurnameTest()
        {
            employeeTest.Surname = surname;

            Assert.AreEqual(surname, employeeTest.Surname);
        }

        [Test]
        public void EmailTest()
        {
            employeeTest.Email = email;

            Assert.AreEqual(email, employeeTest.Email);
        }

        [Test]
        public void StartHourTest()
        {
            employeeTest.StartHour = startHour;

            Assert.AreEqual(startHour, employeeTest.StartHour);
        }

        [Test]
        public void StartMinuteTest()
        {
            employeeTest.StartMinute = startMinute;

            Assert.AreEqual(startMinute, employeeTest.StartMinute);
        }

        [Test]
        public void EndHourTest()
        {
            employeeTest.EndHour = endHour;

            Assert.AreEqual(endHour, employeeTest.EndHour);
        }

        [Test]
        public void EndMinuteTest()
        {
            employeeTest.EndMinute = endMinute;

            Assert.AreEqual(endMinute, employeeTest.EndMinute);
        }

        [Test]
        public void NotificationsTest()
        {
            employeeTest.Notifications = notifications;
            Assert.AreEqual(notifications, employeeTest.Notifications);
        }

        [Test]
        public void DatePasswordChanged()
        {
            employeeTest.DatePasswordChanged = datePasswordChanged;
            Assert.AreEqual(datePasswordChanged, employeeTest.DatePasswordChanged);
        }

        #endregion
    }
}
