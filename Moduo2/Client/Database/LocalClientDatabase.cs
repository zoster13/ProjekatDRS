using ClientCommon.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Client
{
    public class LocalClientDatabase
    {
        //private static LocalClientDatabase localDB;
        private BindingList<Employee> employees;
        private Employee currentEmployee;
        private object locker;

        public LocalClientDatabase()
        {
            employees = new BindingList<Employee>();
            currentEmployee = new Employee();
            locker = new object();
        }

        //public static LocalClientDatabase Instance
        //{
        //    get
        //    {
        //        if (localDB == null)
        //            localDB = new LocalClientDatabase();

        //        return localDB;
        //    }
        //    set
        //    {
        //        if (localDB == null)
        //            localDB = value;
        //    }
        //}

        public BindingList<Employee> Employees
        {
            get
            {
                return employees;
            }
            set
            {
                employees = value;
            }
        }

        public Employee CurrentEmployee
        {
            get
            {
                return currentEmployee;
            }
            set
            {
                currentEmployee = value;
            }
        }

        public object Locker
        {
            get { return this.locker; }
            set { this.locker = value; }
        }

    }
}
