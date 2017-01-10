using ClientCommon.Data;
using System.Collections.Generic;
using System.ServiceModel;
using ICommon;

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
        void ProjectTeamAssign(Project project);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void ReleaseUserStory(UserStory userStory);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void TaskClaimed(Task task);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void TaskCompleted(Task task);

        [OperationContract]
        void AddUserStory(UserStory userStory);

        [OperationContract]
        void AddTask(Task task);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void SendUserStories(List<UserStoryCommon> userStories, string projectName);

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
