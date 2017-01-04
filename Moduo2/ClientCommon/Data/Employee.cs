using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientCommon.Data
{
    public class Employee
    {
        private int id;
        private EmployeeType type;
        private string name;
        private string surname;
        private string email;
        private string password;

        public Employee()
        {

        }

        public Employee(EmployeeType type, string name, string surname, string email, string password)
        {
            this.type = type;
            this.name = name;
            this.surname = surname;
            this.email = email;
            this.password = password;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
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
            get { return this.surname; }
            set { this.surname = value; }
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

        public override string ToString()
        {
            return Name + Surname + ", " + Type;
        }
    }
}
