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
        private static ClientDatabase clientDB;

        private BindingList<Employee> employees;
        //dodati BindingList kompanija
        //dodati BindingList projekata

        private static ClientDatabase instance; //singletone

        private string username; //da pamti username ulogovanog radnika

        public ClientDatabase() 
        {
            employees = new BindingList<Employee>();
        }

        public static ClientDatabase Instance()
        {
            if (instance == null)
                instance = new ClientDatabase();

            return instance;
        }

        //public static ClientDatabase Instance
        //{
        //    get
        //    {
        //        if (clientDB == null)
        //            clientDB = new ClientDatabase();

        //        return clientDB;
        //    }
        //    set
        //    {
        //        if (clientDB == null)
        //            clientDB = value;
        //    }
        //}

        public BindingList<Employee> Employees 
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
