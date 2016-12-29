using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany.DatabaseAccess
{
    class HiringCompanyDB : IHiringCompanyDB
    {
        private static IHiringCompanyDB myDB;

        public static IHiringCompanyDB DBInstance
        {
            get
            {
                if (myDB == null)
                    myDB = new HiringCompanyDB();

                return myDB;
            }
            set
            {
                if (myDB == null)
                    myDB = value;
            }
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
    }
}
