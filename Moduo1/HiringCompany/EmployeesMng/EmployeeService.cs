using EmployeeCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany.EmployeesMng
{
    public class EmployeeService : IEmployeeService
    {
        public void LogIn()
        {
            Console.WriteLine("EmployeeService.LogIn() called ");
        }
    }
}
