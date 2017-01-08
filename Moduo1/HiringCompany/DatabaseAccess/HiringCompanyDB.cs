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
        public object AllEmployees_lock = new object();

        // razmisliti da li da pravite novu klasu za ove in-memory podatke...
        private List<Employee> onlineEmployees;
        private List<Employee> allEmployees;
        Dictionary<string, IEmployeeServiceCallback> connectionChannels;
        //morace biti ovakav dictionary i za konekcije sa drugim serverom

        private HiringCompanyDB()
        {
            onlineEmployees=new List<Employee>();
            allEmployees = new List<Employee>();
            connectionChannels = new Dictionary<string, IEmployeeServiceCallback>();
        }

        public List<Employee> OnlineEmployees
        { 
            get { return onlineEmployees; } 
            set { onlineEmployees = value; } 
        }

        public List<Employee> AllEmployees
        {
            get { return allEmployees; }
            set { allEmployees = value; }
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
            lock (AllEmployees_lock)
            {
                allEmployees.Clear();
            }

            try
            {
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    lock (AllEmployees_lock)
                    {
                        allEmployees.Add(em);
                    }
                }
                
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
            }

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
