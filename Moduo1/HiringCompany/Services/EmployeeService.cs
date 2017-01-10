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
using ICommon;

namespace HiringCompany.Services
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
      ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class EmployeeService : IEmployeeService
    {
        HiringCompanyDB hiringCompanyDB = HiringCompanyDB.Instance();

        OutsorcingCompProxy outsorcingProxy;
        System.Timers.Timer lateOnJobTimer = new System.Timers.Timer();

        public EmployeeService()
        {
            lateOnJobTimer.Elapsed += new ElapsedEventHandler(NotifyOnLate);
            lateOnJobTimer.Interval = 15000;
            //lateOnJobTimer.Enabled = true;
        }

        // slanje maila onima koji nisu online, srediti ovu metodu body i content od maila...
        private void NotifyOnLate(object sender, ElapsedEventArgs e)
        {
            string _senderEmailAddress = "mzftn123fakultet@gmail.com"; // i ovo cuvati u nekom fajlu
            string _senderPassword = "miljanazvezdana123";
            Console.WriteLine("alarm...");
            foreach (Employee em in hiringCompanyDB.AllEmployees)
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

                IEmployeeServiceCallback outCallback;
                if (!hiringCompanyDB.ConnectionChannelsClients.TryGetValue(username, out outCallback))
                {
                    hiringCompanyDB.ConnectionChannelsClients.Add(username, callback);

                    lock (hiringCompanyDB.Employees_lock)
                    {
                        hiringCompanyDB.OnlineEmployees.Add(employee);
                    }

                    SyncAll();
                }
                else
                {
                    Console.WriteLine("Greska kod sign-in, uslo u else...");
                }

            }
            else
            {
                return false;
            }

            return true;
        }

        public void SignOut(string username)
        {
            Employee employee = null;
            Console.WriteLine("SignOut {0} called", username); // for debug
            lock (hiringCompanyDB.Employees_lock)
            {
                Console.WriteLine("SignOut {0} lock", username);


                foreach (Employee e in hiringCompanyDB.OnlineEmployees)
                {
                    Console.WriteLine("SignOut {0} foreach", username);

                    if (e.Username.Equals(username))
                    {
                        Console.WriteLine("SignOut {0} equals", username);
                        employee = e;
                        break;
                    }
                }

                hiringCompanyDB.OnlineEmployees.Remove(employee);
                Console.WriteLine("SignOut {0} removed online ", username);

                hiringCompanyDB.ConnectionChannelsClients.Remove(username);
                Console.WriteLine("SignOut {0} removed channel", username);
            }

            SyncAll();
        }

        public void ChangeEmployeeData(string username, string name, string surname, string email, string password)
        {
            try
            {
                using (var access = new AccessDB())
                {
                    Employee em = access.employees.SingleOrDefault(e => e.Username.Equals(username));

                    if (em != null)
                    {
                        em.Name = name != "" ? name : em.Name;
                        em.Surname = surname != "" ? surname : em.Surname;
                        em.Email = email != "" ? email : em.Email;
                        em.Password = password != "" ? password : em.Password;
                        access.SaveChanges();
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

            lock (hiringCompanyDB.Employees_lock)
            {

                Employee em = hiringCompanyDB.OnlineEmployees.Find(e => e.Username.Equals(username));
                if (em != null)
                {
                    em.Name = name != "" ? name : em.Name;
                    em.Surname = surname != "" ? surname : em.Surname;
                    em.Email = email != "" ? email : em.Email;
                    em.Password = password != "" ? password : em.Password;
                }
            }

            SyncAll();
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

            SyncAll();
        }

        public void AskForPartnership(string outsorcingCompanyName)
        {
            string outsorcingSvcEndpoint = string.Format("net.tcp://{0}/OutsourcingService", hiringCompanyDB.PartnerCompaniesAddresses[outsorcingCompanyName]);

            NetTcpBinding binding = new NetTcpBinding();
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(outsorcingSvcEndpoint));
            IOutsourcingServiceCallback callback = new OutsorcingServiceCallback();
            InstanceContext instanceContext = new InstanceContext(callback);


            // izbrisati iz liste, i dodati ako negde nesto treba
            outsorcingProxy = new OutsorcingCompProxy(instanceContext, binding, endpointAddress);
            // sacuvati proxy, namestiti lockovabhe
            hiringCompanyDB.ConnectionChannelsCompanies.Add(outsorcingCompanyName, outsorcingProxy);

            outsorcingProxy.AskForPartnership(hiringCompanyDB.CompanyName);
        }

        public bool AddNewEmployee(Employee em)
        {
            bool retVal = hiringCompanyDB.AddNewEmployee(em);
            SyncAll();
            return retVal;
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

            SyncAll();
        }

        public void ProjectOverview()
        {
            throw new NotImplementedException();
        }

        public void CreateNewProject(Project p)
        {
            hiringCompanyDB.AddNewProject(p); // onaj neki lock

            try
            {
                // lepse napisati, sa linq
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    if (em.Type == EmployeeType.CEO)
                    {
                        foreach (KeyValuePair<string, IEmployeeServiceCallback> pair in hiringCompanyDB.ConnectionChannelsClients)
                        {
                            if (pair.Key.Equals(em.Username))
                            {
                                // videti da organizujes ovo, neka metoda nesto ili konstruktor
                                CurrentData cData = new CurrentData();
                                cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForCeoApproval;
                                cData.ProjectsForSendingData = hiringCompanyDB.ProjectsForSendingToOutsC;
                                cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
                                cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
                                cData.CompaniesData = hiringCompanyDB.PartnerCompanies;
                                cData.NamesOfCompaniesData = hiringCompanyDB.PartnerCompaniesAddresses.Keys.ToList();

                                pair.Value.SyncData(cData);
                                pair.Value.Notify(string.Format("Project {0} is waiting for approval.", p.Name));
                                //treba napraviti metodu koja notifikuje CEO da treba da potvrdi projekat
                            }
                        }
                        //break;
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
            //lock(hiringCompanyDB.ProjectsForApproval_lock)
            //{
            // foreach(Project proj in hiringCompanyDB.ProjectsForCeoApproval)
            // {
            // if(proj.Name.Equals(p.Name))
            //  {
            // hiringCompanyDB.ProjectsForCeoApproval.Remove(proj);

            try
            {
                var access = new AccessDB();
                var project = from proj in access.projects
                              where proj.Name.Equals(p.Name)
                              select proj;

                var pr = project.ToList().FirstOrDefault();

                pr.IsAcceptedCEO = true;
                access.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }



            try
            {
                bool isNotificationSent = false;
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    if (em.Type == EmployeeType.CEO)
                    {
                        foreach (KeyValuePair<string, IEmployeeServiceCallback> pair in hiringCompanyDB.ConnectionChannelsClients)
                        {
                            CurrentData cData = new CurrentData();
                            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForCeoApproval;
                            cData.ProjectsForSendingData = hiringCompanyDB.ProjectsForSendingToOutsC;
                            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
                            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
                            cData.CompaniesData = hiringCompanyDB.PartnerCompanies;
                            cData.NamesOfCompaniesData = hiringCompanyDB.PartnerCompaniesAddresses.Keys.ToList();
                            pair.Value.SyncData(cData);

                            if (!isNotificationSent)
                            {
                                if (pair.Key.Equals(p.ProductOwner))
                                {
                                    try
                                    {

                                        pair.Value.Notify(string.Format("Project {0} is approved.", p.Name));
                                        isNotificationSent = true;
                                    }
                                    catch (Exception)
                                    {

                                        throw;
                                    }
                                }
                            }
                        }
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

        private void SyncAll()
        {
            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForCeoApproval;
            cData.ProjectsForSendingData = hiringCompanyDB.ProjectsForSendingToOutsC;
            cData.CompaniesData = hiringCompanyDB.PartnerCompanies;
            cData.NamesOfCompaniesData = hiringCompanyDB.PartnerCompaniesAddresses.Keys.ToList();

            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannelsClients.Values)
            {
                call.SyncData(cData);
            }
        }

    }
}
