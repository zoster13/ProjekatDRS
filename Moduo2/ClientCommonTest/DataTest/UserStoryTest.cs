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
    public class UserStoryTest
    {
        private UserStory userStoryTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.userStoryTest = new UserStory();
        }
    }
}
