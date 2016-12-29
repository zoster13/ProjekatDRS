using EmployeeCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany.DatabaseAccess
{
    interface IHiringCompanyDB
    {
        bool AddNewEmployee(Employee employee);
    }
}
