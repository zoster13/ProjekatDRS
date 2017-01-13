using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon;
using System.ComponentModel;
using EmployeeCommon.Data;

namespace Client
{
    public class ClientDatabase
    {
        private BindingList<Employee> employees;
        private BindingList<Employee> allEmployees;
        private BindingList<Project> projects;
        private BindingList<PartnerCompany> companies;

        private BindingList<string> namesOfCompanies;

        private BindingList<Project> projectsForApproval;
        private BindingList<Project> projectsForSending;
        private BindingList<Project> projectsForClosing;

        // singletone
        private static ClientDatabase instance; 

        // current user data
        private string username = string.Empty; 
        private string password = string.Empty;

        private MainWindow main;

        private ClientDatabase() 
        {
            employees = new BindingList<Employee>();
            allEmployees = new BindingList<Employee>();
            projects = new BindingList<Project>();
            companies = new BindingList<PartnerCompany>();
            namesOfCompanies = new BindingList<string>();

            projectsForApproval = new BindingList<Project>();
            projectsForSending = new BindingList<Project>();
            projectsForClosing = new BindingList<Project>();
        }

        public static ClientDatabase Instance()
        {
            if (instance == null)
            {
                instance = new ClientDatabase();
            }
            return instance;
        }

        public BindingList<Employee> Employees 
        {
            get { return employees; }
            set { employees = value; }
        }

        public BindingList<Employee> AllEmployees
        {
            get { return allEmployees; }
            set { allEmployees = value; }
        }

        public BindingList<Project> ProjectsInDevelopment 
        {
            get { return projects; }
            set { projects = value; }
        }

        public BindingList<PartnerCompany> Companies
        {
            get { return companies; }
            set { companies = value; }
        }

        public BindingList<string> NamesOfCompanies 
        {
            get { return namesOfCompanies; }
            set { namesOfCompanies = value; }
        }

        public BindingList<Project> ProjectsForApproval
        {
            get { return projectsForApproval; }
            set { projectsForApproval = value; }
        }

        public BindingList<Project> ProjectsForSending 
        {
            get { return projectsForSending; }
            set { projectsForSending = value; }
        }

        public BindingList<Project> ProjectsForClosing
        {
            get { return projectsForClosing; }
            set { projectsForClosing = value; }
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

        public MainWindow Main 
        {
            get { return main; }
            set { main = value; }
        }
    }
}
