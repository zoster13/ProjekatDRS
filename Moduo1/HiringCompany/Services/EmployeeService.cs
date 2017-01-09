using EmployeeCommon;
using HiringCompany.DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace HiringCompany.Services
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
      ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class EmployeeService : IEmployeeService
    {
        HiringCompanyDB hiringCompanyDB = HiringCompanyDB.Instance();

        System.Timers.Timer lateOnJobTimer = new System.Timers.Timer();

        public EmployeeService()
        {
            lateOnJobTimer.Elapsed += new ElapsedEventHandler(NotifyOnLate);
            lateOnJobTimer.Interval = 15000; 
            //lateOnJobTimer.Enabled = true;
        }

        private void NotifyOnLate(object sender, ElapsedEventArgs e)
        {
            string _senderEmailAddress = "mzftn123fakultet@gmail.com";
            string _senderPassword = "miljanazvezdana123";
            Console.WriteLine("alarm...");
            foreach (Employee em in hiringCompanyDB.AllEmployees) // slanje maila onima koji nisu online
            {
                if (!hiringCompanyDB.OnlineEmployees.Contains(em))
                {
                    DateTime current = DateTime.Now;
                    DateTime workTimeEmployee = new DateTime(current.Year, current.Month, current.Day, em.StartHour, em.StartMinute, 0);
                    TimeSpan timeDiff = current - workTimeEmployee;
                    TimeSpan allowed = new TimeSpan(0, 15, 0);

                    if (timeDiff > allowed)
                    {
                        var client = new SmtpClient("smtp.gmail.com", 587)
                        {
                            Credentials = new NetworkCredential(_senderEmailAddress, _senderPassword),
                            EnableSsl = true
                        };
                        client.Send(_senderEmailAddress, em.Email, "Obavestenje", "Zakasnili ste na posao!");
                        /*{System.Net.WebException: Unable to connect to the remote server ---> System.Net.Sockets.SocketException: A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond 64.233.184.109:587
   at System.Net.Sockets.Socket.DoConnect(EndPoint endPointSnapshot, SocketAddress socketAddress)
   at System.Net.ServicePoint.ConnectSocketInternal(Boolean connectFailure, Socket s4, Socket s6, Socket& socket, IPAddress& address, ConnectSocketState state, IAsyncResult asyncResult, Exception& exception)
   --- End of inner exception stack trace ---
   at System.Net.ServicePoint.GetConnection(PooledStream PooledStream, Object owner, Boolean async, IPAddress& address, Socket& abortSocket, Socket& abortSocket6)
   at System.Net.PooledStream.Activate(Object owningObject, Boolean async, GeneralAsyncDelegate asyncCallback)
   at System.Net.PooledStream.Activate(Object owningObject, GeneralAsyncDelegate asyncCallback)
   at System.Net.ConnectionPool.GetConnection(Object owningObject, GeneralAsyncDelegate asyncCallback, Int32 creationTimeout)
   at System.Net.Mail.SmtpConnection.GetConnection(ServicePoint servicePoint)
   at System.Net.Mail.SmtpTransport.GetConnection(ServicePoint servicePoint)
   at System.Net.Mail.SmtpClient.GetConnection()
   at System.Net.Mail.SmtpClient.Send(MailMessage message)}*/
                    }
                }
            }
        }



        public bool SignIn(string username, string password)
        {
            Console.WriteLine("EmployeeService.LogIn() called ");

            Employee employee = hiringCompanyDB.GetEmployee(username);

            if (employee != null && password.Equals(employee.Password))
            {
                IEmployeeServiceCallback callback = OperationContext.Current.GetCallbackChannel<IEmployeeServiceCallback>();
                hiringCompanyDB.ConnectionChannels.Add(username, callback); // kad ne ugasis server, a klijent se ponovo poveze, puca posto taj username postoji
                // nesto se desi, kad apredugo ceka na klijenta i klijent se ugasi ne izbrise ga...

                lock (hiringCompanyDB.Employees_lock)
                {
                    hiringCompanyDB.OnlineEmployees.Add(employee);
                }
                CurrentData cData = new CurrentData();
                cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
                cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
                cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;

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
            Employee employee = null; // namestiti...

            lock (hiringCompanyDB.Employees_lock)
            {
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
            }
            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;
            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
            {
                call.SyncData(cData);
            }

        }

        public void ChangeEmployeeData(string username, string name, string surname, string email, string password)
        {
            try
            {
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    if (em.Username == username)
                    {
                        if (name != "")
                        {
                            em.Name = name;
                        }
                        if (surname != "")
                        {
                            em.Surname = surname;
                        }
                        if (email != "")
                        {
                            em.Email = email;
                        }
                        if (password != "")
                        {
                            em.Password = password;
                        }

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

            lock (hiringCompanyDB.AllEmployees_lock)
            {
                foreach (Employee em in hiringCompanyDB.AllEmployees)
                {
                    if (em.Username == username)
                    {
                        if (name != "")
                        {
                            em.Name = name;
                        }
                        if (surname != "")
                        {
                            em.Surname = surname;
                        }
                        if (email != "")
                        {
                            em.Email = email;
                        }
                        if (password != "")
                        {
                            em.Password = password;
                        }
                        break;
                    }
                }
            }

            lock (hiringCompanyDB.Employees_lock)
            {
                foreach (Employee em in hiringCompanyDB.OnlineEmployees)
                {
                    if (em.Username == username)
                    {
                        if (name != "")
                        {
                            em.Name = name;
                        }
                        if (surname != "")
                        {
                            em.Surname = surname;
                        }
                        if (email != "")
                        {
                            em.Email = email;
                        }
                        if (password != "")
                        {
                            em.Password = password;
                        }
                        break;
                    }
                }
            }

            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;

            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
            {
                call.SyncData(cData);
            }
        }

        public void SetWorkingHours(string username, int beginH, int beginM, int endH, int endM)
        {
            try
            {
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    if (em.Username == username)
                    {
                        em.StartHour = beginH;
                        em.StartMinute = beginM;
                        em.EndHour = endH;
                        em.EndMinute = endM;

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

            lock (hiringCompanyDB.Employees_lock)
            {
                foreach (Employee em in hiringCompanyDB.OnlineEmployees)
                {
                    if (em.Username == username)
                    {
                        em.StartHour = beginH;
                        em.StartMinute = beginM;
                        em.EndHour = endH;
                        em.EndMinute = endM;
                        break;
                    }
                }
            }

            lock (hiringCompanyDB.AllEmployees_lock)
            {
                foreach (Employee em in hiringCompanyDB.AllEmployees)
                {
                    if (em.Username == username)
                    {
                        em.StartHour = beginH;
                        em.StartMinute = beginM;
                        em.EndHour = endH;
                        em.EndMinute = endM;
                        break;
                    }
                }
            }

            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;

            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
            {
                call.SyncData(cData);
            }
        }

        public void AskForPartnership()
        {
            throw new NotImplementedException();
        }

        public void AddNewEmployee(Employee em)
        {
            hiringCompanyDB.AddNewEmployee(em);

            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;

            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
            {
                call.SyncData(cData);
            }
        }

        public void ChangeEmployeeType(string username, EmployeeType type)
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

            lock (hiringCompanyDB.Employees_lock)
            {
                foreach (Employee em in hiringCompanyDB.OnlineEmployees)
                {
                    if (em.Username == username)
                    {
                        em.Type = type;
                        break;
                    }
                }
            }

            lock (hiringCompanyDB.AllEmployees_lock)
            {
                foreach (Employee em in hiringCompanyDB.AllEmployees)
                {
                    if (em.Username == username)
                    {
                        em.Type = type;
                        break;
                    }
                }
            }

            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;

            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
            {
                call.SyncData(cData);
            }
        }

        public void ProjectOverview()
        {
            throw new NotImplementedException();
        }

        public void CreateNewProject(Project p)
        {
            try
            {
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    if (em.Type == EmployeeType.CEO)
                    {
                        foreach (KeyValuePair<string, IEmployeeServiceCallback> pair in hiringCompanyDB.ConnectionChannels)
                        {
                            if (pair.Key.Equals(em.Username))
                            {
                                lock (hiringCompanyDB.ProjectsForApproval_lock)
                                {
                                    hiringCompanyDB.ProjectsForApproval.Add(p);
                                }

                                CurrentData cData = new CurrentData();
                                cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;
                                cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
                                cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
                                pair.Value.SyncData(cData);
                                //treba napraviti metodu koja notifikuje CEO da treba da potvrdi projekat
                            }
                        }
                        break;
                    }
                }
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


        public void ProjectApproved(Project p) // treba pozvati NotifyPO
        {
            var callback = hiringCompanyDB.ConnectionChannels[p.ProductOwner];
            try
            {
                callback.NotifyPO();
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }
    }
}
