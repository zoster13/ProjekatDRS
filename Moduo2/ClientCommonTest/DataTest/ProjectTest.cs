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
    public class ProjectTest
    {
        private Project projectTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.projectTest = new Project();
        }
    }
}
