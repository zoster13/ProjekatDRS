using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon;

namespace Client
{
    public class ClientDatabase
    {
        private static ClientDatabase clientDB;

        private List<Employee> employees;

        private string username; //da pamti username ulogovanog radnika

        public ClientDatabase() 
        {
            employees = new List<Employee>();
        }

        public static ClientDatabase Instance
        {
            get
            {
                if (clientDB == null)
                    clientDB = new ClientDatabase();

                return clientDB;
            }
            set
            {
                if (clientDB == null)
                    clientDB = value;
            }
        }

        public List<Employee> Employees 
        {
            get { return employees; }
            set { employees = value; }
        }

        public string Username 
        {
            get { return username; }
            set { username = value; }
        }
    }
}
