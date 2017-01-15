using ICommon;
using NUnit.Framework;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    [TestFixture]
    public class OutsourcingServiceTest
    {
        OutsourcingService outsourcingServiceTest;

        List<UserStoryCommon> commonUserStories;
        UserStoryCommon usc1 = new UserStoryCommon() { Title = "usc1" };
        UserStoryCommon usc2 = new UserStoryCommon() { Title = "usc2" };


        [OneTimeSetUp]
        public void SetupTest()
        {
            outsourcingServiceTest = new OutsourcingService();
            commonUserStories = new List<UserStoryCommon>() { usc1, usc2 };
        }

        //AskForPartnership
        [Test]
        public void AskForPartnershipTest()
        {
            outsourcingServiceTest.AskForPartnership("hiringCompany13");
        }

        //SendEvaluatedUserstoriesToOutsourcingCompany
        [Test]
        public void SendEvaluatedUserstoriesToOutsourcingCompanyTest()
        {
            outsourcingServiceTest.SendEvaluatedUserstoriesToOutsourcingCompany(commonUserStories, "proj1");
        }

        //SendProjectToOutsourcingCompany
        [Test]
        public void SendProjectToOutsourcingCompanyTest()
        {
            outsourcingServiceTest.SendProjectToOutsourcingCompany("hiringCompany13", new ProjectCommon());
        }
    }
}
