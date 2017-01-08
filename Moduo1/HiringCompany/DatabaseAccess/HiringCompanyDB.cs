using EmployeeCommon;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany.DatabaseAccess
{
    public class HiringCompanyDB 
    {
        private static HiringCompanyDB myDB;

        public object Employees_lock = new object();

        // razmisliti da li da pravite novu klasu za ove in-memory podatke...
        private List<Employee> onlineEmployees;
        Dictionary<string, IEmployeeServiceCallback> connectionChannels;
        //morace biti ovakav dictionary i za konekcije sa drugim serverom

        private HiringCompanyDB()
        {
            onlineEmployees=new List<Employee>();
            connectionChannels = new Dictionary<string, IEmployeeServiceCallback>();
        }

        public List<Employee> OnlineEmployees
        { 
            get { return onlineEmployees; } 
            set { onlineEmployees = value; } 
        }
        public Dictionary<string,IEmployeeServiceCallback> ConnectionChannels
        {
            get { return connectionChannels; }
            set { connectionChannels = value; }
        }

        public static HiringCompanyDB Instance()
        {
            if (myDB == null)
                myDB = new HiringCompanyDB();
            return myDB;
        }

        public Employee GetEmployee(string username)
        {
            using (var access = new AccessDB())
            {
                var employee = from em in access.employees
                               where em.Username.Equals(username)
                               select em;

                return employee.ToList().FirstOrDefault();
            }
        }
    }
}
