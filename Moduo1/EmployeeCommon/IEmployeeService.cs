using EmployeeCommon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon
{
    [ServiceContract(SessionMode = SessionMode.Required,
        CallbackContract = typeof(IEmployeeServiceCallback))]
    public interface IEmployeeService
    {
        // All users
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        bool SignIn(string username,string password);
         
        [OperationContract( IsInitiating = false, IsTerminating = true)] 
        void SignOut(string username);

        [OperationContract]
        void ChangeEmployeeData(string username, string name, string surname, string email, string password);

        [OperationContract]
        void SetWorkingHours(string username, int beginH, int beginM, int endH, int endM);

        // CEO
        [OperationContract]
        void AskForPartnership(string companyName);

        // CEO, HR
        [OperationContract]
        bool AddNewEmployee(Employee e);

        [OperationContract]
        void ChangeEmployeeType(string username,EmployeeType type);

        // CEO, PO, SM
        [OperationContract]
        void SendApprovedUserStories(string projectName, List<UserStory> userStories);

        // PO
        [OperationContract]
        void CreateNewProject(Project p);

        // Notification methods
        [OperationContract]
        void ProjectApprovedByCeo(Project p);

        [OperationContract]
        void SendProject(string outscCompany, Project p);

        [OperationContract]
        void CloseProject(string projectName);
    }
}
