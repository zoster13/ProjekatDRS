using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon;
using System.ComponentModel;

namespace Client
{
    public class ClientDatabase
    {
        public object Employees_lock = new object();

        private BindingList<Employee> employees;
        private BindingList<Project> projects;
        //dodati BindingList kompanija

        private static ClientDatabase instance; //singletone

        private string username; //da pamti username ulogovanog radnika

        private ClientDatabase() 
        {
            employees = new BindingList<Employee>();
            projects = new BindingList<Project>();
        }

        public static ClientDatabase Instance()
        {
            if (instance == null)
                instance = new ClientDatabase();

            return instance;
        }

        public BindingList<Employee> Employees 
        {
            get { return employees; }
            set { employees = value; }
        }

        public BindingList<Project> Projects 
        {
            get { return projects; }
            set { projects = value; }
        }

        public string Username 
        {
            get { return username; }
            set { username = value; }
        }
    }
}
