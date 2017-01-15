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
using ICommon;
using System.ServiceModel;

namespace HiringCompanyTest.Services
{
    [TestFixture]
    public class HiringServiceTest
    {
        private HiringService hiringServiceTest;
        private Project p;
        private Project p2;
        private ProjectCommon pc;
        private ProjectCommon pc2;
        private UserStoryCommon usC;
        private UserStoryCommon usC2;
        private List<UserStoryCommon> userStoriesC = new List<UserStoryCommon>();
        private UserStory us;
        private UserStory us2;
        private List<UserStory> userStories = new List<UserStory>();
        private PartnerCompany partnerCompanyTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            hiringServiceTest = new HiringService();
            HiringCompanyDB.Instance = Substitute.For<IHiringCompanyDB>();

            partnerCompanyTest = new PartnerCompany("druga");

            List<string> partnerCompanies = new List<string>();
            partnerCompanies.Add(partnerCompanyTest.Name);
            HiringCompanyDB.Instance.GetPartnerCompaniesNames().Returns(partnerCompanies);
            //AccessDB access = new AccessDB();
            InternalDatabase iDB = InternalDatabase.Instance();
            iDB.PossiblePartnersAddresses.Add("pokusaj", "localhost:9998");
            

            
            Employee e = new Employee("amisic", "789", EmployeeType.CEO, "Aleksandra", "Misic", "amisic@gmail.com", 9, 10, 17, 30);
            iDB.OnlineEmployees.Add(e);

            IEmployeeServiceCallback callbackClient = Substitute.For<IEmployeeServiceCallback>();
            ICommunicationObject cObject = callbackClient as ICommunicationObject;
            iDB.ConnectionChannelsClients.Add("amisic", callbackClient);

            //userStories
            us = new UserStory("prva", "ndinsj", "nsjdndsijds");
            us2 = new UserStory("druga", "sdnsido", "njdsinsj");
            userStories.Add(us);
            userStories.Add(us2);

            usC = new UserStoryCommon("prva", "ndinsj", "nsjdndsijds", true);
            usC2 = new UserStoryCommon("druga", "sdnsido", "njdsinsj", true);
            userStoriesC.Add(usC);
            userStoriesC.Add(usC2);

            //Project
            p = new Project("pokusaj", "nsjdndskjd", "amisic", "rzekanovic");
            p.UserStories = userStories;
            HiringCompanyDB.Instance.AddNewProject(p);

            p2 = new Project("pokusaj2", "nsjdndskjd", "amisic", "rzekanovic");
            HiringCompanyDB.Instance.AddNewProject(p2);

            //ProjectCommon
            pc = new ProjectCommon("pokusaj", "nsjdndskjd", DateTime.Now, DateTime.Now);
            pc2 = new ProjectCommon("pokusaj2", "nsjdndskjd", DateTime.Now, DateTime.Now);

            //HiringCompanyDB mock metode
            HiringCompanyDB.Instance.ResponseForProjectRequestFieldsChange("bluc", pc).Returns(true);
            HiringCompanyDB.Instance.ResponseForProjectRequestFieldsChange("bluc", pc2).Returns(false);

            HiringCompanyDB.Instance.SendUserStoriesToHiringCompanyFieldsChange(userStories, p.Name);
            HiringCompanyDB.Instance.SendClosedUserStoryFieldChange(p.Name, "prva").Returns(true);

            //mozda treba za metodu  ResponseForPartnershipRequest
            HiringCompanyDB.Instance.AddNewPartnerCompany(new PartnerCompany("cekic")).Returns(true); 
        }

        [Test]
        [TestCase(true, "pokusaj")]
        public void ResponseForPartnershipRequestTestAccepted(bool accepted, string outsourcingCompName)
        {
            Assert.DoesNotThrow(() =>
            {
                hiringServiceTest.ResponseForPartnershipRequest(accepted, outsourcingCompName);
            });
        }

        [Test]
        [TestCase(false, "bluc")]
        public void ResponseForPartnershipRequestTestNotAccepted(bool accepted, string outsourcingCompName)
        {
            Assert.DoesNotThrow(() =>
            {
                hiringServiceTest.ResponseForPartnershipRequest(accepted, outsourcingCompName);
            });
        }

        [Test]
        public void ResponseForProjectRequestTestAccepted()
        {
            
            pc.IsAcceptedByOutsCompany = true;

            Assert.DoesNotThrow(() =>
            {
                hiringServiceTest.ResponseForProjectRequest("bluc", pc);
            });
        }


        [Test]
        public void ResponseForProjectRequestTestDeclined()
        {

            pc2.IsAcceptedByOutsCompany = false;

            Assert.DoesNotThrow(() =>
            {
                hiringServiceTest.ResponseForProjectRequest("bluc", pc2);
            });
        }

        [Test]
        public void SendUserStoriesToHiringCompanyTest()
        {
            Assert.DoesNotThrow(() =>
            {
                hiringServiceTest.SendUserStoriesToHiringCompany(userStoriesC, p.Name);
            });
        }

        [Test]
        public void SendClosedUserStoryTest()
        {
            Assert.DoesNotThrow(() =>
            {
                hiringServiceTest.SendClosedUserStory(p.Name, "prva");
            });
        }
    }
}
