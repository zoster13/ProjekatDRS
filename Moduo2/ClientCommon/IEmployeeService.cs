using ClientCommon.Data;
using System.Collections.Generic;
using System.ServiceModel;

namespace ClientCommon
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICallbackMethods))]
    public interface IEmployeeService
    {
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void LogIn(string email, string password);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void LogOut(Employee employee);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void AddTeam(Team team);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void EditEmployeeData(Employee employee);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void ProjectTeamAssign(string projName, string teamName);   // tim treba da ima listu projekata koji su mu dodeljeni
                                                                    // treba izvaditi iz baze projekat i staviti ga u tim
                                                                    // onda poslati svim online clanovima tog tima taj projekat

        [OperationContract]
        List<Employee> GetAllOnlineEmployees();

        [OperationContract]
        List<Employee> GetAllEmployees();

        [OperationContract]
        List<Team> GetAllTeams();

        [OperationContract]
        List<HiringCompany> GetAllHiringCompanies();

        [OperationContract]
        void AddEmployee(Employee employee);

        [OperationContract]
        void UpdateEmployeeFunctionAndTeam(Employee employee, string newTeamName);

        [OperationContract]
        void ResponseToPartnershipRequest(bool accepted, string companyName);
    }
}
