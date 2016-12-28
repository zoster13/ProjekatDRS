using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCommon.Data
{
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

        public EmployeeType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Surname
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Email
        {
            get { return this.email; }
            set { this.email = value; }
        }
        
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }
    }
}
