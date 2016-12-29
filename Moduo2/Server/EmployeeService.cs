using ClientCommon;
using ClientCommon.Data;
using Server.Access;
using Server.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class EmployeeService : IEmployeeService
    {
        public List<Employee> GetAllEmployees()
        {
            //var result = EmployeeServiceDatabase.Instance.GetAllEmployees();
            return InternalDatabase.Instance.OnlineEmployees;
        }

        public Employee LogIn(string email, string password)
        {
            Employee employee = EmployeeServiceDatabase.Instance.GetEmployee(email);
            
            if(employee!=null && password.Equals(employee.Password))
            {
                InternalDatabase.Instance.OnlineEmployees.Add(employee);
                return employee;
            }
            else
            {
                return null;
            }
        }

        public bool LogOut(string email)
        {
            Employee employee = null;

            foreach(Employee e in InternalDatabase.Instance.OnlineEmployees)
            {
                if(e.Email.Equals(email))
                {
                    employee = e;
                }
            }

            bool removed = InternalDatabase.Instance.OnlineEmployees.Remove(employee);
            return removed;
        }
    }
}