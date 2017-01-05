using ClientCommon.Data;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Server.Access
{
    public class EmployeeServiceDatabase : IEmployeeServiceDatabase
    {
        private static IEmployeeServiceDatabase myDB;

        public static IEmployeeServiceDatabase Instance
        {
            get
            {
                if(myDB == null)
                {
                    myDB = new EmployeeServiceDatabase();
                }

                return myDB;
            }

            set
            {
                if(myDB ==null)
                {
                    myDB = value;
                }
            }
        }

        public bool AddEmployee(Employee employee)
        {
            using (var access = new AccessDB())
            {
                access.Employees.Add(employee);
                int i = access.SaveChanges();

                if (i > 0)
                    return true;
                return false;
            }
        }

        //public List<Employee> GetAllEmployees()
        //{
        //    using (var access = new AccessDB())
        //    {
        //        return access.Employees.ToList();
        //    }
        //}

        public Employee GetEmployee(string email)
        {
            using (var access = new AccessDB())
            {
                var employee = from em in access.Employees.Include(e => e.Team)
                               where em.Email.Equals(email)
                               select em;

                return employee.ToList().First();
            }
        }

        public bool AddTeam(Team team)
        {
            using (var access = new AccessDB())
            {
                var team1 = from t in access.Teams
                           where t.Name.Equals(team.Name)
                           select t;

                if (team1 == null)
                {
                    access.Teams.Add(team);
                    int i = access.SaveChanges();

                    if (i > 0)
                        return true;
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public Team GetTeam(string name)
        {
            using (var access = new AccessDB())
            {
                var team = from t in access.Teams
                               where t.Name.Equals(name)
                               select t;

                return team.ToList().First();
            }
        }

    }
}
