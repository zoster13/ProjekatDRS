using ICommon;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace ServerTest.ServerProxy
{
    [TestFixture]
    public class ServerProxyTest
    {
        private IHiringService hiringServiceTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            hiringServiceTest = Substitute.For<IHiringService>();
         
        }

        [Test]
        [TestCase(true, "vega it")]
        [TestCase(true, "dms")]
        [TestCase(false, "rt-rk")]

        public void ResponseForPartnershipRequestTest(bool accepted, string osName)
        {
            Assert.DoesNotThrow(() => hiringServiceTest.ResponseForPartnershipRequest(accepted, osName));
        }

        [Test]
        [TestCase("vega it")]
        [TestCase("dms")]
        [TestCase("rt-rk")]

        public void ResponseForProjectRequestTest(string osName)
        {
            ProjectCommon prCommon = new ProjectCommon() { Name = "proj" };

            Assert.DoesNotThrow(() => hiringServiceTest.ResponseForProjectRequest(osName, prCommon));
        }

        [Test]
        [TestCase("us1", "proj1")]
        [TestCase("us2", "proj1")]
        [TestCase("us3", "proj1")]
        public void SendClosedUserStoryTest(string title, string projecName)
        {
            Assert.DoesNotThrow(() => hiringServiceTest.SendClosedUserStory(title, projecName));
        }

        [Test]
        [TestCase("proj1")]
        [TestCase("proj2")]
        [TestCase("proj3")]
        public void SendUserStoriesToHiringCompanyTest(string projName)
        {
            Assert.DoesNotThrow(() => hiringServiceTest.SendUserStoriesToHiringCompany(new List<UserStoryCommon>(), projName));
        }
    }
}
