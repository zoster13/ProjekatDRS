using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon
{
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

        private Employee(string eUsername, string ePassword, EmployeeType eType, string eName, string eSurname, string eEmail) 
        {
            name = eUsername;
            password = ePassword;
            type = eType;
            name = eName;
            surname = eSurname;
            email = eSurname;
        }

        public string Username 
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public EmployeeType Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }
    }
}
