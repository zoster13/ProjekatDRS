﻿using EmployeeCommon;
using HiringCompany.DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany.EmployeesMng
{
  
      [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class EmployeeService : IEmployeeService
    {
          HiringCompanyDB hiringCompanyDB = HiringCompanyDB.Instance();
   

        public bool SignIn(string username,string password)
        {
            Console.WriteLine("EmployeeService.LogIn() called ");

            Employee employee = hiringCompanyDB.GetEmployee(username);

            if (employee != null && password.Equals(employee.Password))
            {
                IEmployeeServiceCallback callback = OperationContext.Current.GetCallbackChannel<IEmployeeServiceCallback>();
                hiringCompanyDB.ConnectionChannels.Add(username, callback);

                hiringCompanyDB.OnlineEmployees.Add(employee);

                // pokuasj update-a...
                CurrentData cData = new CurrentData();
                cData.EmployeesData = hiringCompanyDB.OnlineEmployees;

                // pozivanje synca na callback objectu..
                foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values) 
                {
                    call.SyncData(cData);
                }
                //callback.SyncData(cData);

               // return employee;
            }
            else
            {
                return false;
            }

            return true;
        }

        public void SignOut()
        {
            throw new NotImplementedException();
        }

        public void ListOnlineEmployees()
        {
            throw new NotImplementedException();
        }

        public void ListOutsorcingCompanies()
        {
            throw new NotImplementedException();
        }

        public void ChangeEmployeeData()
        {
            throw new NotImplementedException();
        }

        public void SetWorkingHours()
        {
            throw new NotImplementedException();
        }

        public void AskForPartnership()
        {
            throw new NotImplementedException();
        }

        public void AddNewEmployee()
        {
            throw new NotImplementedException();
        }

        public void ChangeEmployeeType()
        {
            throw new NotImplementedException();
        }

        public void ProjectOverview()
        {
            throw new NotImplementedException();
        }

        public void CreateNewProject()
        {
            throw new NotImplementedException();
        }
    }
}
