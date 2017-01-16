using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientCommon.TempStructure;

namespace ClientCommonTest.TempStructureTest
{
    [TestFixture]
    public class TaskAndUserStoryCompletedFlagTest
    {
        private TaskAndUserStoryCompletedFlag test;

        [OneTimeSetUp]
        public void SetupTest()
        {
            this.test = new TaskAndUserStoryCompletedFlag();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new TaskAndUserStoryCompletedFlag());
        }

        [Test]
        public void TaskTest()
        {
            ClientCommon.Data.Task task = new ClientCommon.Data.Task() { Title = "task" };
            test.Task = task;

            Assert.AreEqual(test.Task, task);
        }

        [Test]
        public void UserStoryCompletedFlagTest()
        {
            test.UserStoryCompletedFlag = true;

            Assert.AreEqual(test.UserStoryCompletedFlag, true);
        }
    }
}
