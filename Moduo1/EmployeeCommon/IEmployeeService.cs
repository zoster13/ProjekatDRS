﻿using System;
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

        [OperationContract] // /*(IsOneWay=true)*/ razmisliti za ostale metode
        void ListOnlineEmployees();

        [OperationContract]
        void ListOutsorcingCompanies();

        [OperationContract]
        void ChangeEmployeeData();

        [OperationContract]
        void SetWorkingHours();

        // CEO
        [OperationContract]
        void AskForPartnership();

        // CEO, HR
        [OperationContract]
        void AddNewEmployee();

        [OperationContract]
        void ChangeEmployeeType();

        // CEO, PO, SM
        [OperationContract]
        void ProjectOverview();

        //PO
        [OperationContract]
        void CreateNewProject();

        //[OperationContract]
        //void DefineUserStories(); // ?
    }
}
