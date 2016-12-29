using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon
{
    [DataContract]
    public class Employee 
    {
        private string username;
        private string password;
        private EmployeeType type;
        private string name;
        private string surname;
        private string email;

        public Employee() 
        {

        }

        public Employee(string eUsername, string ePassword, EmployeeType eType, string eName, string eSurname, string eEmail) 
        {
            name = eUsername;
            password = ePassword;
            type = eType;
            name = eName;
            surname = eSurname;
            email = eSurname;
        }

        [DataMember]
        public string Username 
        {
            get { return username; }
            set { username = value; }
        }

        [DataMember]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [DataMember]
        public EmployeeType Type
        {
            get { return type; }
            set { type = value; }
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }

        [DataMember]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
    }
}
