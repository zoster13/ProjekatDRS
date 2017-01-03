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
        private BindingList<PartnerCompany> companies;

        private static ClientDatabase instance; //singletone

        private string username; //da pamti username ulogovanog radnika

        private MainWindow main;

        private ClientDatabase() 
        {
            employees = new BindingList<Employee>();
            projects = new BindingList<Project>();
            companies = new BindingList<PartnerCompany>();
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

        public BindingList<PartnerCompany> Companies
        {
            get { return companies; }
            set { companies = value; }
        }

        public string Username 
        {
            get { return username; }
            set { username = value; }
        }

        public MainWindow Main 
        {
            get { return main; }
            set { main = value; }
        }
    }
}
