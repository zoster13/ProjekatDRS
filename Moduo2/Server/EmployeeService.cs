using ClientCommon;
using ClientCommon.Data;
using Server.Access;
using Server.Database;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Server
{
    public class EmployeeService : IEmployeeService
    {
        public void LogIn(string email, string password)
        {
            Employee employee = EmployeeServiceDatabase.Instance.GetEmployee(email);

            if(employee!=null && password.Equals(employee.Password))
            {
                InternalDatabase.Instance.OnlineEmployees.Add(employee);

                Publisher.Instance.LogInCallback(employee);
            }
        }

        public void LogOut(string email)
        {
            Employee employee = null;
            
            foreach (Employee e in InternalDatabase.Instance.OnlineEmployees)
            {
                if(e.Email.Equals(email))
                {
                    employee = e;
                    InternalDatabase.Instance.OnlineEmployees.Remove(e);
                    break;
                }
            }
            
            Publisher.Instance.LogOutCallback(employee);
        }
        
        public List<Employee> GetAllOnlineEmployees()
        {
            return InternalDatabase.Instance.OnlineEmployees;
        }

        public List<Employee> GetAllEmployees()
        {
            using (var access = new AccessDB())
            {
                return access.Employees.ToList();
            }
        }

        public List<Team> GetAllTeams()
        {
            return InternalDatabase.Instance.Teams;
        }

        public List<HiringCompany> GetAllHiringCompanies()
        {
            throw new NotImplementedException();
        }

        public void AddTeam(Team team)
        {
            //Employee teamLeader = EmployeeServiceDatabase.Instance.GetEmployee(team.TeamLeaderEmail);
            
            //if(teamLeader == null)
            //{
            //    //team.TeamLeader.Team = team;
            //    EmployeeServiceDatabase.Instance.AddEmployee(team.TeamLeader);
            //}
            //else
            //{
            //    EmployeeServiceDatabase.Instance.UpdateEmployeeFunction(teamLeader.Email, (short)EmployeeType.TEAMLEADER);
            //    team.TeamLeader = teamLeader;
            //}
            
            if (EmployeeServiceDatabase.Instance.AddTeam(team))
            {
                InternalDatabase.Instance.Teams.Add(team);

                Publisher.Instance.TeamAddedCallback(team, true);
            }
            else
            {
                Publisher.Instance.TeamAddedCallback(team, false);
            }
        }

        public void EditEmployeeData(Employee employee)
        {
            EmployeeServiceDatabase.Instance.UpdateEmployee(employee);

            Publisher.Instance.EditEmployeeCallback(employee);
        }

        public void ProjectTeamAssign(string projName, string teamName)
        {
            throw new NotImplementedException();
        }

        public void AddEmployee(Employee employee)
        {
            EmployeeServiceDatabase.Instance.AddEmployee(employee);

            Publisher.Instance.AddEmployeeCallback(employee);
        }
    }
}