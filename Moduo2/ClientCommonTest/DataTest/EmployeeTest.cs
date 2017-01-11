using ClientCommon.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ClientCommonTest.DataTest
{
    [TestFixture]
    public class EmployeeTest
    {
        private Employee employeeTest;
        private EmployeeType type = EmployeeType.CEO;
        private string name = "Marko";
        private string surname = "Markovic";
        private string email = "marko@gmail.com";
        private string password = "mare123";
        private int id = 1;
        private Team team = new Team("SaaS");
        private DateTime someTime = DateTime.Now;
        
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
            Assert.DoesNotThrow(() => new Employee(type, name, surname, email, password, team));

            employeeTest = new Employee(type, name, surname, email, password, team);
            Assert.AreEqual(type, employeeTest.Type);
            Assert.AreEqual(name, employeeTest.Name);
            Assert.AreEqual(surname, employeeTest.Surname);
            Assert.AreEqual(email, employeeTest.Email);
            Assert.AreEqual(password, employeeTest.Password);
            Assert.AreEqual(team, employeeTest.Type);
        }

        [Test]
        public void IdTest()
        {
            employeeTest.Id = id;

            Assert.AreEqual(id, employeeTest.Id);
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
        public void PasswordTest()
        {
            employeeTest.Password = password;

            Assert.AreEqual(password, employeeTest.Password);
        }

        [Test]
        public void TeamTest()
        {
            employeeTest.Team = team;

            Assert.AreEqual(employeeTest.Team, team);
        }

        [Test]
        public void WorkingHoursStartTest()
        {
            employeeTest.WorkingHoursStart = someTime;

            Assert.AreEqual(employeeTest.WorkingHoursStart, someTime);
        }

        [Test]
        public void WorkingHoursEndTest()
        {
            employeeTest.WorkingHoursEnd = someTime;

            Assert.AreEqual(employeeTest.WorkingHoursEnd, someTime);
        }

        [Test]
        public void PasswordTimeStampTest()
        {
            employeeTest.PasswordTimeStamp = someTime;

            Assert.AreEqual(employeeTest.PasswordTimeStamp, someTime);
        }

        #endregion Tests
    }
}
