using ClientCommon.Data;
using System.Collections.Generic;

namespace Server.Access
{
    public interface IEmployeeServiceDatabase
    {
        bool AddEmployee(Employee employee);
        List<Employee> GetAllEmployees();

    }
}
