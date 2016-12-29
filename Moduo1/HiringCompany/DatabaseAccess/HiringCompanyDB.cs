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

        // razmisliti da li da pravite novu klasu za ove in-memory podatke...
        private List<Employee> onlineEmployees;
        Dictionary<string, IEmployeeServiceCallback> connectionChannels;
        
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

        public bool AddNewEmployee(EmployeeCommon.Employee employee)
        {
            try
            {
                var access = new AccessDB();
                access.employees.Add(employee);
                int i = access.SaveChanges();

                if (i > 0)
                    return true;
                return false;
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
            return false;
            //using (var access = new AccessDB())
            //{
            //    access.employees.Add(employee);
            //    int i = access.SaveChanges();

            //    if (i > 0)
            //        return true;
            //    return false;
            //}
        }

        public Employee GetEmployee(string username)
        {
            using (var access = new AccessDB())
            {
                var employee = from em in access.employees
                               where em.Username.Equals(username)
                               select em;

                return employee.ToList().First();
            }
        }
    }
}
