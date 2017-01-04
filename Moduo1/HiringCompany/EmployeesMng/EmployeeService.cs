using EmployeeCommon;
using HiringCompany.DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
                CurrentData cData = new CurrentData();
                cData.EmployeesData = hiringCompanyDB.OnlineEmployees;

                foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values) 
                {
                    call.SyncData(cData);
                }
               
               // return employee;
            }
            else
            {
                return false;
            }

            return true;
        }

        public void SignOut(string username)
        {
             Employee employee=null; // namestiti...

         
                 // sacuvati podatke tog korisnika u bazi
                 foreach (Employee e in hiringCompanyDB.OnlineEmployees)
                 {
                     if (e.Username.Equals(username))
                     {
                         employee = e;
                         break;
                     }
                 }

                 hiringCompanyDB.OnlineEmployees.Remove(employee);
                 hiringCompanyDB.ConnectionChannels.Remove(username);

                 CurrentData cData = new CurrentData();
                 cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
                 foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
                 {
                     call.SyncData(cData);
                 }
             
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

        public void AddNewEmployee(Employee em)
        {
            try
            {
                var access = new AccessDB();
                access.employees.Add(em); //dodati proveru postoji li vec korisnik sa tim username-om
                access.SaveChanges();
                //ne mora se raditi sync jer se u klijentskojDB ne cuvaju svi korisnici,vec samo online
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
            }
        }

        public void ChangeEmployeeType(string username,EmployeeType type)
        {
            try
            {
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    if (em.Username == username)
                    {
                        em.Type = type;
                        break;
                    }
                }
                access.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
            }

            foreach (Employee em in hiringCompanyDB.OnlineEmployees)
            {
                if (em.Username == username)
                {
                    em.Type = type;
                    break;
                }
            }

            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;

            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
            {
                call.SyncData(cData);
            }
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
