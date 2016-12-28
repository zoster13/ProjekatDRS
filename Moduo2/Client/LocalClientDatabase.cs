using ClientCommon.Data;
using System.Collections.ObjectModel;

namespace Client
{
    public class LocalClientDatabase
    {
        private static LocalClientDatabase localDB;

        private ObservableCollection<Employee> employees;

        public LocalClientDatabase()
        {
            employees = new ObservableCollection<Employee>();
        }

        public static LocalClientDatabase Instance
        {
            get
            {
                if (localDB == null)
                    localDB = new LocalClientDatabase();

                return localDB;
            }
            set
            {
                if (localDB == null)
                    localDB = value;
            }
        }

        public ObservableCollection<Employee> Employees
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
    }
}
