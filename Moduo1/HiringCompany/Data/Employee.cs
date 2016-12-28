using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany
{
    public enum EmployeeType
    {
        CEO = 0,
        ENG,        //engineer
        TL,         //team leader
        SM          // scrum master
    }

    public class Employee 
    {
        private string username;
        private string password;
        private EmployeeType type;
        private string name;
        private string surname;
        private string email;

        private Employee() 
        {

        }
    }
}
