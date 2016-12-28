using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientCommon.Data;

namespace Client
{
    public class LocalClientDatabase
    {
        private static LocalClientDatabase localDB;

        private List<Employee> employees;

        public LocalClientDatabase()
        {
            employees = new List<Employee>();
        }

        public static LocalClientDatabase Instance
        {
            get
            {
                if (localDB == null)
                    localDB = new LocalClientDatabase();

                return localDB;
            }
            set
            {
                if (localDB == null)
                    localDB = value;
            }
        }

        public List<Employee> Employees
        {
            get
            {
                return employees;
            }
            set
            {
                employees = value;
            }
        }
    }
}
