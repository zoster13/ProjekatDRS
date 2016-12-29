using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany.DatabaseAccess
{
    class HiringCompanyDB : IHiringCompanyDB
    {
        private static IHiringCompanyDB myDB;

        public static IHiringCompanyDB DBInstance
        {
            get
            {
                if (myDB == null)
                    myDB = new HiringCompanyDB();

                return myDB;
            }
            set
            {
                if (myDB == null)
                    myDB = value;
            }
        }

        public bool AddNewEmployee(EmployeeCommon.Employee employee)
        {
            using (var access = new AccessDB())
            {
                access.employees.Add(employee);
                int i = access.SaveChanges();

                if (i > 0)
                    return true;
                return false;
            }
        }
    }
}
