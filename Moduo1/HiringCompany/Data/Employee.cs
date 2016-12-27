using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany
{
    public enum EmployeeType
    {
        CEO=0,
        PO,
        SM,
        HR
    }

    public class Employee
    {
        private EmployeeType type;
        private string name;
        private string surname;
        private string email;
        private string password;


        private Employee() 
        {

        }
    }
}
