//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using HiringCompany.DatabaseAccess;
//using EmployeeCommon.Data;
//using NSubstitute;

//namespace HiringCompanyTest.DatabaseAccessTest
//{
//    [TestFixture]
//    public class HiringCompanyDBTest
//    {
//        //private HiringCompanyDB dbTest;
//        private IHiringCompanyDB dbInstanceTest;
//        private bool result;
//        private Employee em;
//        private Employee em2;

//        [OneTimeSetUp]
//        public void SetupTest()
//        {
//            dbInstanceTest = Substitute.For<IHiringCompanyDB>();

//            em = new Employee("zklasnic", "456", EmployeeType.CEO, "Zvezdana", "Klasnic", "zklasnic94@gmail.com", 9, 0, 17, 0);
//            em2 = new Employee("jjovanovic", "112", EmployeeType.HR, "Jovan", "Jovanovic", "jjovanovic@gmail.com", 10, 0, 18, 0);
//            //dbInstanceTest.AddNewEmployee(Arg.Is<Employee>(em)).Returns(true);
//            //dbInstanceTest.AddNewEmployee(Arg.Is<Employee>(em2)).Returns(false);


//            //HiringCompanyDB.Instance = Substitute.For<IHiringCompanyDB>();
//            dbInstanceTest.AddNewEmployee(Arg.Is<Employee>(e => e.Username == "jjovanovic")).Returns(true);
//            dbInstanceTest.AddNewEmployee(Arg.Is<Employee>(e => e.Username != "jjovanovic")).Returns(false);
//            dbInstanceTest.AddNewPartnerCompany(Arg.Is<PartnerCompany>(pc => pc.Name == "DMS")).Returns(true);
//            dbInstanceTest.AddNewPartnerCompany(Arg.Is<PartnerCompany>(pc => pc.Name != "DMS")).Returns(false);
//            dbInstanceTest.AddNewProject(Arg.Is<Project>(p => p.Name == "PrviProjekat")).Returns(true);
//            dbInstanceTest.AddNewProject(Arg.Is<Project>(p => p.Name != "PrviProjekat")).Returns(false);

//            //dbInstanceTest.AddNewEmployee(Arg.Is<Employee>(e => e.Username == "jjovanovic")).Returns(true);
//            //dbInstanceTest.AddNewEmployee(Arg.Is<Employee>(e => e.Username != "jjovanovic")).Returns(false);
//            //dbInstanceTest.AddNewPartnerCompany(Arg.Is<PartnerCompany>(pc => pc.Name == "DMS")).Returns(true);
//            //dbInstanceTest.AddNewPartnerCompany(Arg.Is<PartnerCompany>(pc => pc.Name != "DMS")).Returns(false);
//            //dbInstanceTest.AddNewProject(Arg.Is<Project>(p => p.Name == "PrviProjekat")).Returns(true);
//            //dbInstanceTest.AddNewProject(Arg.Is<Project>(p => p.Name != "PrviProjekat")).Returns(false);
//        }

//        //[Test]
//        //public void DbAccess_lockTest()
//        //{
//        //    object obj = new object();
//        //    dbInstanceTest.DbAccess_lock = obj;

//        //    Assert.AreEqual(obj, dbInstanceTest.DbAccess_lock);
//        //}

//        //[Test]
//        //public void AllEmployees_lockTest()
//        //{
//        //    object obj = new object();
//        //    dbInstanceTest.AllEmployees_lock = obj;

//        //    Assert.AreEqual(obj, dbInstanceTest.AllEmployees_lock);
//        //}

//        //[Test]
//        //public void Projects_lockTest()
//        //{
//        //    object obj = new object();
//        //    dbInstanceTest.Projects_lock = obj;

//        //    Assert.AreEqual(obj, dbInstanceTest.Projects_lock);
//        //}

//        //[Test]
//        //public void PartnerCompanies_lockTest()
//        //{
//        //    object obj = new object();
//        //    dbInstanceTest.PartnerCompanies_lock = obj;

//        //    Assert.AreEqual(obj, dbInstanceTest.PartnerCompanies_lock);
//        //}

//        [Test]
//        public void AddNewEmployeeTestGood()
//        {
//            Employee employee = new Employee() { Username = "jjovanovic" };
//            result = dbInstanceTest.AddNewEmployee(employee);
//            //result = dbInstanceTest.AddNewEmployee(em);

//            Assert.IsTrue(result);
//        }

//        [Test]
//        public void AddNewEmployeeTestFault()
//        {
//            Employee employee = new Employee() { Username = "djovic" };
//            result = dbInstanceTest.AddNewEmployee(employee);
//            //result = dbInstanceTest.AddNewEmployee(em2);

//            Assert.IsFalse(result);
//        }

//        //[Test]
//        //public void AddNewPartnerCompanyTestGood()
//        //{
//        //    PartnerCompany partnerCompany = new PartnerCompany() { Name = "DMS" };
//        //    result = dbInstanceTest.AddNewPartnerCompany(partnerCompany);

//        //    Assert.IsTrue(result);
//        //}

//        //[Test]
//        //public void AddNewPartnerCompanyTestFault()
//        //{
//        //    PartnerCompany partnerCompany = new PartnerCompany() { Name = "RTRK" };
//        //    result = dbInstanceTest.AddNewPartnerCompany(partnerCompany);

//        //    Assert.IsFalse(result);
//        //}

//        //[Test]
//        //public void AddNewProjectTestGood()
//        //{
//        //    Project project = new Project() { Name = "PrviProjekat" };
//        //    result = dbInstanceTest.AddNewProject(project);

//        //    Assert.IsTrue(result);
//        //}

//        //[Test]
//        //public void AddNewProjectTestFault()
//        //{
//        //    Project project = new Project() { Name = "DrugiProjekat" };
//        //    result = dbInstanceTest.AddNewProject(project);

//        //    Assert.IsFalse(result);
//        //}
//    }
//}

