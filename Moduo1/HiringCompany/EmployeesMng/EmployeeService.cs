using EmployeeCommon;
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
        public bool SignIn()
        {
            Console.WriteLine("EmployeeService.LogIn() called ");
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
