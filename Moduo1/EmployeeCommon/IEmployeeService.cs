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
        bool SignIn(); // da li da se u ovom LogIn-u interno implementira i subscribe? Da korisnik dobija podatke koji su mu od znacaja dok je logovan
         
        [OperationContract( IsInitiating = false, IsTerminating = true)]
        void SignOut();   

        [OperationContract/*(IsOneWay=true)*/]
        void ListOnlineEmployees();

        [OperationContract/*(IsOneWay=true)*/]
        void ListOutsorcingCompanies();

        [OperationContract/*(IsOneWay=true)*/]
        void ChangeEmployeeData();

        [OperationContract/*(IsOneWay=true)*/]
        void SetWorkingHours();

        // CEO
        [OperationContract/*(IsOneWay=true)*/]
        void AskForPartnership();

        // CEO, HR
        [OperationContract/*(IsOneWay=true)*/]
        void AddNewEmployee();

        [OperationContract/*(IsOneWay=true)*/]
        void ChangeEmployeeType();

        // CEO, PO, SM
        [OperationContract/*(IsOneWay=true)*/]
        void ProjectOverview();

        //PO
        [OperationContract/*(IsOneWay=true)*/]
        void CreateNewProject();

        //[OperationContract/*(IsOneWay=true)*/]
        //void DefineUserStories(); // ?
    }
}
