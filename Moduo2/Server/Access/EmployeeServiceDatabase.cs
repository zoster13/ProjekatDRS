using ClientCommon.Data;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.SqlClient;

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

        public void UpdateEmployee(string email, short type)
        {
            string commandText = "UPDATE Employees SET Type = @type Where Email=@email";

            string constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EmployeeServiceDB.mdf;Integrated Security=True";
            SqlConnection con = new SqlConnection(constr);
            SqlCommand updateCommand = new SqlCommand(commandText, con);
            updateCommand.Parameters.AddWithValue("@type", type.ToString());
            updateCommand.Parameters.AddWithValue("@email", email);

            con.Open();
            updateCommand.ExecuteNonQuery();
            con.Close();
        }

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
