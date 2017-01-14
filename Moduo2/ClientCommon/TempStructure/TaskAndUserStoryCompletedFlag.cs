using ClientCommon.Data;

namespace ClientCommon.TempStructure
{
    public class TaskAndUserStoryCompletedFlag
    {
        private Task task;
        private bool userStoryCompletedFlag;

        public TaskAndUserStoryCompletedFlag()
        {
        }

        public Task Task
        {
            get { return task; }
            set { task = value; }
        }

        public bool UserStoryCompletedFlag
        {
            get { return userStoryCompletedFlag; }
            set { userStoryCompletedFlag = value; }
        }
    }
}
