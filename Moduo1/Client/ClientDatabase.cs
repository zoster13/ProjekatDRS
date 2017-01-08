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
        public object AllEmployees_lock = new object();
        public object Projects_lock = new object();
        public object ProjectsForApproval_lock = new object();

        private BindingList<Employee> employees;
        private BindingList<Employee> allEmployees;
        private BindingList<Project> projects;
        private BindingList<PartnerCompany> companies; //ovo je lista partneskih kompanija
        //trebace i da se napravi lista kompanija koje jos uvak nisu partnerske,vec su samo konektovane sa nasim serverom

        private BindingList<Project> projectsForApproval;

        private static ClientDatabase instance; //singletone

        private string username=string.Empty; //da pamti username ulogovanog radnika

        private MainWindow main;

        private ClientDatabase() 
        {
            employees = new BindingList<Employee>();
            allEmployees = new BindingList<Employee>();
            projects = new BindingList<Project>();
            companies = new BindingList<PartnerCompany>();

            projectsForApproval = new BindingList<Project>();
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

        public BindingList<Employee> AllEmployees
        {
            get { return allEmployees; }
            set { allEmployees = value; }
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

        public BindingList<Project> ProjectsForApproval
        {
            get { return projectsForApproval; }
            set { projectsForApproval = value; }
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
