﻿using HiringCompany.DatabaseAccess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiringCompany;

namespace HiringCompanyTest.DatabaseAccessTest
{
    [TestFixture]
    public class AccessDBTest
    {
        private AccessDB accessDBTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.accessDBTest = new AccessDB();
        }

        [Test]
        public void EmployeeTest()
        {
            Assert.DoesNotThrow(() => accessDBTest.Employees.FirstOrDefault());
        }

        [Test]
        public void ProjectsTest()
        {
            Assert.DoesNotThrow(() => accessDBTest.Projects.FirstOrDefault());
        }

        [Test]
        public void CompaniesTest()
        {
            Assert.DoesNotThrow(() => accessDBTest.Companies.FirstOrDefault());
        }
    }
}

