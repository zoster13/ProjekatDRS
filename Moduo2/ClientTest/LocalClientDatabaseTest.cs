using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;
using NUnit.Framework;

namespace ClientTest
{
    public class LocalClientDatabaseTest
    {
        private LocalClientDatabase lcb;

        [OneTimeSetUp]
        public void ConstructorTest()
        {
            
        }

        public void InstanceTest()
        {
            lcb = LocalClientDatabase.Instance;
        }
    }
}
