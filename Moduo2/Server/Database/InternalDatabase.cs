using ClientCommon.Data;
using System.Collections.Generic;

namespace Server.Database
{
    public class InternalDatabase
    {
        private static InternalDatabase instance;
        private static List<Employee> onlineEmployees;

        private InternalDatabase()
        {
            onlineEmployees = new List<Employee>();
        }

        public static InternalDatabase Instance
        {
            get
            {
                if (instance == null)
                    instance = new InternalDatabase();

                return instance;
            }
            set
            {
                if (instance == null)
                    instance = value;
            }
        }

        public List<Employee> OnlineEmployees
        {
            get { return onlineEmployees; }
            set { onlineEmployees = value; }
        }
    }
}
