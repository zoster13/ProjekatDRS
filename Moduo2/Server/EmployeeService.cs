using ClientCommon;
using ClientCommon.Data;
using Server.Access;
using Server.Database;
using System.Collections.Generic;

namespace Server
{
    public class EmployeeService : IEmployeeService
    {
        public void LogIn(string email, string password)
        {
            Employee employee = EmployeeServiceDatabase.Instance.GetEmployee(email);
            
            if(employee!=null && password.Equals(employee.Password))
            {
                InternalDatabase.Instance.OnlineEmployees.Add(employee);

                Publisher.Instance.LogInCallback(employee);
            }
        }

        public void LogOut(string email)
        {
            Employee employee = null;
            Employee employeeCopy = null;
            
            foreach (Employee e in InternalDatabase.Instance.OnlineEmployees)
            {
                if(e.Email.Equals(email))
                {
                    employee = e;
                    employeeCopy = e;
                }
            }

            InternalDatabase.Instance.OnlineEmployees.Remove(employee);

            Publisher.Instance.LogOutCallback(employeeCopy);
        }

        public List<Employee> GetAllEmployees()
        {
            return InternalDatabase.Instance.OnlineEmployees;
        }
    }
}