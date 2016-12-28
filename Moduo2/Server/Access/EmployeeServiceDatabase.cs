using ClientCommon.Data;
using System.Collections.Generic;
using System.Linq;

namespace Server.Access
{
    public class EmployeeServiceDatabase : IEmployeeServiceDatabase
    {
        private static IEmployeeServiceDatabase myDB;

        public static IEmployeeServiceDatabase Instance
        {
            get
            {
                if(myDB == null)
                {
                    myDB = new EmployeeServiceDatabase();
                }

                return myDB;
            }

            set
            {
                if(myDB ==null)
                {
                    myDB = value;
                }
            }
        }

        public bool AddEmployee(Employee employee)
        {
            using (var access = new AccessDB())
            {
                access.Employees.Add(employee);
                int i = access.SaveChanges();

                if (i > 0)
                    return true;
                return false;
            }
        }

        public List<Employee> GetAllEmployees()
        {
            using (var access = new AccessDB())
            {
                return access.Employees.ToList();
            }
        }

    }
}
