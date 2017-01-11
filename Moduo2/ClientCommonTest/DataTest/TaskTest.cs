using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientCommon.Data;

namespace ClientCommonTest.DataTest
{
    [TestFixture]
    public class TaskTest
    {
        private ClientCommon.Data.Task taskTest;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.taskTest = new ClientCommon.Data.Task();
        }
    }
}
