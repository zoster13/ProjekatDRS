using ClientCommon.Data;
using System.Collections.Generic;

namespace Server.Database
{
    public class InternalDatabase
    {
        private static List<Employee> onlineEmployeesDB;

        private InternalDatabase()
        {
            onlineEmployeesDB = new List<Employee>();
        }

        public static List<Employee> Instance
        {
            get
            {
                if (onlineEmployeesDB == null)
                    onlineEmployeesDB = new List<Employee>();

                return onlineEmployeesDB;
            }
            set
            {
                if (onlineEmployeesDB == null)
                    onlineEmployeesDB = value;
            }
        }
    }
}
