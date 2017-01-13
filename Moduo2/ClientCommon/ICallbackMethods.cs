using ClientCommon.Data;
using ICommon;
using System.Collections.Generic;
using System.ServiceModel;

namespace ClientCommon
{
    [ServiceContract]
    public interface ICallbackMethods
    {
        [OperationContract(IsOneWay = true)]
        [ServiceKnownType(typeof(Employee))]
        void LogInCallback(Employee employee, bool loggedIn);

        [OperationContract(IsOneWay = true)]
        void LogOutCallback(Employee employee);

        [OperationContract(IsOneWay = true)]
        void TeamAddedCallback(Team team);

        [OperationContract(IsOneWay = true)]
        void AddTeamAndTLCallback(Team team, Employee teamLeader);

        [OperationContract(IsOneWay = true)]
        void TypeChangeCallback(Team team, EmployeeType newType);

        [OperationContract(IsOneWay = true)]
        void EditEmployeeCallback(Employee employee);

        [OperationContract(IsOneWay = true)]
        void AddEmployeeCallback(Employee employee);

        [OperationContract(IsOneWay = true)]
        void NotifyJustMe(Employee employee);

        [OperationContract(IsOneWay = true)]
        void ScrumMasterAddedCallback(Employee employee, Team team);

        [OperationContract(IsOneWay = true)]
        void ProjectTeamAssignCallback(Project project);

        [OperationContract(IsOneWay = true)]
        void ReceiveUserStoriesCallback(List<UserStoryCommon> commUserStories, string projectName);

        [OperationContract(IsOneWay = true)]
        void ReleaseUserStoryCallback(UserStory userStory);

        [OperationContract(IsOneWay = true)]
        void TaskClaimedCallback(Task task);

        [OperationContract(IsOneWay = true)]
        void TaskCompletedCallback(Task task);

        //delegiranje zahtjeva
        [OperationContract(IsOneWay = true)]
        void SendNotificationToCEO(Notification notification);

        [OperationContract(IsOneWay = true)]
        void ResponseToPartnershipRequestCallback(HiringCompany hiringCompany);
    }
}
