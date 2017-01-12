using ClientCommon.Data;
using System.Collections.Generic;

namespace Server.Database
{
    public class InternalDatabase
    {
        private static InternalDatabase instance;
        private static List<Employee> onlineEmployees;
        private object lockerOnlineEmployees;
        
        private InternalDatabase()
        {
            onlineEmployees = new List<Employee>();
            lockerOnlineEmployees = new object();
        }

        public static InternalDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InternalDatabase();
                }
                return instance;
            }
            set
            {
                if (instance == null)
                {
                    instance = value;
                }
            }
        }

        public List<Employee> OnlineEmployees
        {
            get { return onlineEmployees; }
            set { onlineEmployees = value; }
        }
        
        public object LockerOnlineEmployees
        {
            get { return this.lockerOnlineEmployees; }
            set { lockerOnlineEmployees = value; }
        }
    }
}
