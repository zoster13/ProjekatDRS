using ClientCommon.Data;
using System.Collections.Generic;
using System.ServiceModel;
using ICommon;

namespace ClientCommon
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICallbackMethods))]
    public interface IEmployeeService
    {
        [OperationContract(IsOneWay = true, IsInitiating = true)]
        void LogIn(string email, string password);

        [OperationContract(IsOneWay = true, IsTerminating = true)]
        void LogOut(Employee employee);

        [OperationContract(IsOneWay = true)]
        void AddTeam(Team team);

        [OperationContract(IsOneWay = true)]
        void AddTeamAndTL(Team team, Employee teamLeader);

        [OperationContract(IsOneWay = true)]
        void EditEmployeeData(Employee employee);

        [OperationContract(IsOneWay = true)]
        void ProjectTeamAssign(Project project);

        [OperationContract(IsOneWay = true)]
        void ReleaseUserStory(UserStory userStory);

        [OperationContract(IsOneWay = true)]
        void TaskClaimed(Task task);

        [OperationContract(IsOneWay = true)]
        void TaskCompleted(Task task);

        [OperationContract]
        void AddUserStory(UserStory userStory, string projectName);

        [OperationContract]
        void AddTask(Task task);

        [OperationContract(IsOneWay = true)]
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
        
        //Response to Hiring company
        [OperationContract]
        void ResponseToPartnershipRequest(bool accepted, string hiringCompanyName);

        [OperationContract]
        void ResponseToProjectRequest(bool accepted, Project project);
    }
}
