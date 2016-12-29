using ClientCommon;
using ClientCommon.Data;
using Server.Access;
using Server.Database;
using System;
using System.Collections.Generic;

namespace Server
{
    public class EmployeeService : IEmployeeService
    {
        public List<Employee> GetAllEmployees()
        {
            //var result = EmployeeServiceDatabase.Instance.GetAllEmployees();
            return InternalDatabase.Instance;
        }

        public Employee LogIn(string email, string password)
        {
            Employee employee = EmployeeServiceDatabase.Instance.GetEmployee(email);
            
            if(password.Equals(employee.Password))
            {
                InternalDatabase.Instance.Add(employee);
                return employee;
            }
            else
            {
                return null;
            }
        }

        public bool LogOut(string email)
        {
            Employee employee = EmployeeServiceDatabase.Instance.GetEmployee(email);
            bool removed = InternalDatabase.Instance.Remove(employee);

            if (removed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
