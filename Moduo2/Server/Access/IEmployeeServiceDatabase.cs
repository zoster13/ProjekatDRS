using ClientCommon.Data;
using System.Collections.Generic;

namespace Server.Access
{
    public interface IEmployeeServiceDatabase
    {
        bool AddEmployee(Employee employee);
        //List<Employee> GetAllEmployees();

        Employee GetEmployee(string email);

        bool AddTeam(Team team);

        Team GetTeam(string email);

        void UpdateEmployeeFunctionAndTeam(Employee employee, string newTeamName);

        void UpdateEmployee(Employee employee);
        void UpdateScrumMaster(Employee employee);

        void AddNotification(Notification notification);
    }
}
