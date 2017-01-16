using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using HiringCompany;
using EmployeeCommon;
using HiringCompany.Services;
using EmployeeCommon.Data;
using HiringCompany.DatabaseAccess;
using NSubstitute;
using System.ServiceModel;

namespace HiringCompanyTest
{
    [TestFixture]
    public class NotifierTest
    {
        private Notifier notifierTest;
        //private CurrentData cData;
        private InternalDatabase internalDatabase;
        private PartnerCompany partnerCompanyTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            HiringCompanyDB.Instance = Substitute.For<IHiringCompanyDB>();
            partnerCompanyTest = new PartnerCompany("treca");

            List<string> partnerCompanies = new List<string>();
            partnerCompanies.Add(partnerCompanyTest.Name);
            HiringCompanyDB.Instance.GetPartnerCompaniesNames().Returns(partnerCompanies);
            notifierTest = new Notifier();
            internalDatabase = InternalDatabase.Instance();
            HiringCompanyDB.Instance.AllEmployees().Returns(new List<Employee>());
            HiringCompanyDB.Instance.ProjectsForCeoApproval().Returns(new List<Project>());
            HiringCompanyDB.Instance.ProjectsForSendingToOutsC().Returns(new List<Project>());
            HiringCompanyDB.Instance.ProjectsInDevelopment().Returns(new List<Project>());
            HiringCompanyDB.Instance.PartnerCompanies().Returns(new List<PartnerCompany>());


            IEmployeeServiceCallback callbackClient = Substitute.For<IEmployeeServiceCallback>();

            ICommunicationObject cObject = callbackClient as ICommunicationObject;
            internalDatabase.ConnectionChannelsClients.Add("mvujakovic", callbackClient);
        }

        [Test]
        public void SyncAllTest()
        {
            Assert.DoesNotThrow(() =>
            {
                notifierTest.SyncAll();
            });
        }

        [Test]
        public void SyncSpecialClientTest()
        {
            Assert.DoesNotThrow(() =>
            {
                notifierTest.SyncSpecialClients(EmployeeType.CEO, "");
            });
        }
    }
}
