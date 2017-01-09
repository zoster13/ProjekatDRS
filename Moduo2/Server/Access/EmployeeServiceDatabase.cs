using ClientCommon.Data;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.SqlClient;
using System;

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
                if (employee.Team != null)
                {
                    Team team = access.Teams.FirstOrDefault(t => t.Name.Equals(employee.Team.Name));
                    employee.Team = team;
                }

                access.Employees.Add(employee);
                int i = access.SaveChanges();

                if (i > 0)
                    return true;
                return false;
            }
        }

        public void UpdateEmployeeFunctionAndTeam(Employee employee, string newTeamName)
        {
            using (var access = new AccessDB())
            {
                //Team oldTeam = access.Teams.FirstOrDefault(t => t.Name.Equals(employee.Team.Name));   
                Team newTeam = access.Teams.FirstOrDefault(t => t.Name.Equals(newTeamName));
                employee.Team = newTeam;
                employee.Type = EmployeeType.TEAMLEADER;

                //Update in Database
                string commandText = "UPDATE Employees SET Type = @type, Team_Id = @team Where Email=@email";

                string constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EmployeeServiceDB.mdf;Integrated Security=True";
                SqlConnection con = new SqlConnection(constr);
                SqlCommand updateCommand = new SqlCommand(commandText, con);
                updateCommand.Parameters.AddWithValue("@type", EmployeeType.TEAMLEADER);
                updateCommand.Parameters.AddWithValue("@email", employee.Email);
                updateCommand.Parameters.AddWithValue("@team", newTeam.Id);

                con.Open();
                updateCommand.ExecuteNonQuery();
                con.Close();
            }
        }

        public Employee GetEmployee(string email)
        {
            using (var access = new AccessDB())
            {
                var employee = from em in access.Employees
                               where em.Email.Equals(email)
                               select em;
                try
                {
                    return employee.ToList().First();
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool AddTeam(Team team)
        {
            using (var access = new AccessDB())
            {
                var team1 = from t in access.Teams
                           where t.Name.Equals(team.Name)
                           select t;

                if (team1.ToList().FirstOrDefault() == null)
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

        public void UpdateEmployee(Employee employee)
        {
            string commandText = "UPDATE Employees SET Name = @name, Surname = @surname, WorkingHoursStart = @workingHoursStart, WorkingHoursEnd = @workingHoursEnd, Password = @password Where Email=@email";

            string constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EmployeeServiceDB.mdf;Integrated Security=True";
            SqlConnection con = new SqlConnection(constr);
            SqlCommand updateCommand = new SqlCommand(commandText, con);
            updateCommand.Parameters.AddWithValue("@email", employee.Email);
            updateCommand.Parameters.AddWithValue("@name", employee.Name);
            updateCommand.Parameters.AddWithValue("@surname", employee.Surname);
            updateCommand.Parameters.AddWithValue("@password", employee.Password);
            updateCommand.Parameters.AddWithValue("@workingHoursStart", employee.WorkingHoursStart.ToString());
            updateCommand.Parameters.AddWithValue("@workingHoursEnd", employee.WorkingHoursEnd.ToString());
            
            con.Open();
            updateCommand.ExecuteNonQuery();
            con.Close();
        }

    }
}
