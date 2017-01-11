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
using HiringCompany.Logger;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace HiringCompany.Services
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
      ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class EmployeeService : IEmployeeService
    {
        public static readonly log4net.ILog Logger = LogHelper.GetLogger();

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
            Logger.Info(string.Format("SignIn({0}, {1}) called.", username, password));

            Employee employee = hiringCompanyDB.GetEmployee(username);

            if (employee != null && password.Equals(employee.Password))
            {
                IEmployeeServiceCallback callback = OperationContext.Current.GetCallbackChannel<IEmployeeServiceCallback>();
                IEmployeeServiceCallback outCallback;

                // korisiti?
                ICommunicationObject co = callback as ICommunicationObject;
                Console.WriteLine(co.State.ToString());
                if (co.State == CommunicationState.Opened)
                {

                }

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
                Logger.Info(string.Format("User:{0}, password: {1} SignIn was unsuccessfull.", username, password));
                return false;
            }

            Logger.Info(string.Format("User:{0}, password: {1} SignIn was successfull.", username, password));
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
                using (var access = new AccessDB())
                {
                    Employee em = access.employees.SingleOrDefault(e => e.Username.Equals(username));

                    if (em != null)
                    {
                        em.StartHour = beginH;
                        em.StartMinute = beginM;
                        em.EndHour = endH;
                        em.EndMinute = endM;
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
                    em.StartHour = beginH;
                    em.StartMinute = beginM;
                    em.EndHour = endH;
                    em.EndMinute = endM;
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
            IHiringService callback = new OutsorcingServiceCallback();
            InstanceContext instanceContext = new InstanceContext(callback);


            // izbrisati iz liste, i dodati ako negde nesto treba
            outsorcingProxy = new OutsorcingCompProxy(instanceContext, binding, endpointAddress);
            // sacuvati proxy, namestiti lockovabhe
            hiringCompanyDB.ConnectionChannelsCompanies.Add(outsorcingCompanyName, outsorcingProxy);

            //i ovde ce trebati try catch - ako ne valja kanal

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

                //using (var access = new AccessDB())
                //{

                //}

                // lepse napisati, sa linq
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    if (em.Type == EmployeeType.CEO)
                    {
                        string forRemove = string.Empty;

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
                                cData.ProjectsData = hiringCompanyDB.ProjectsInDevelopment;

                                ICommunicationObject co = pair.Value as ICommunicationObject;
                                Console.WriteLine(co.State.ToString());
                                if (co.State == CommunicationState.Opened)
                                {

                                }

                                try
                                {
                                    pair.Value.SyncData(cData);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    forRemove = pair.Key;
                                }
                                //pair.Value.SyncData(cData);

                                try
                                {
                                    pair.Value.Notify(string.Format("Project {0} is waiting for approval.", p.Name));
                                }
                                catch (Exception e)
                                {

                                    Console.WriteLine(e.Message);
                                    forRemove = pair.Key;
                                }

                                //pair.Value.Notify(string.Format("Project {0} is waiting for approval.", p.Name));

                                break;
                            }

                        }

                        if (!forRemove.Equals(string.Empty))
                        {
                            hiringCompanyDB.ConnectionChannelsClients.Remove(forRemove);
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
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }



            try
            {

                var access = new AccessDB();

                string forRemove = string.Empty;

                SyncAll();

                try
                {
                    hiringCompanyDB.ConnectionChannelsClients[p.ProductOwner].Notify(string.Format("Project {0} is approved.", p.Name));
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                    forRemove = p.ProductOwner;
                }

                if (!forRemove.Equals(string.Empty))
                {
                    hiringCompanyDB.ConnectionChannelsClients.Remove(forRemove);
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

        public void SendProject(string outscCompany, Project p) 
        {
            ProjectCommon proj = new ProjectCommon(p.Name, p.Description, p.StartDate, p.Deadline);
            hiringCompanyDB.ConnectionChannelsCompanies[outscCompany].SendProjectToOutsourcingCompany(hiringCompanyDB.CompanyName, proj);
        }

        // namestiti da kada se desi greska sa klijentske strane da se on uvek disposuje kako treba, tj da se izloguje i da se
        // uradi remove channel uvek sa serverske strane ..ili pre nego sto pozovemo sync da vidimo da li je channel u dobrom stanju i slati ako jeste
        // tj pre poziva bilo koje metode proveravati stanje kanala
        private void SyncAll()
        {
            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForCeoApproval;
            cData.ProjectsForSendingData = hiringCompanyDB.ProjectsForSendingToOutsC;
            cData.CompaniesData = hiringCompanyDB.PartnerCompanies;
            cData.NamesOfCompaniesData = hiringCompanyDB.PartnerCompaniesAddresses.Keys.ToList();
            cData.ProjectsData = hiringCompanyDB.ProjectsInDevelopment;

            List<string> forRemove = new List<string>();

            foreach (var channel in hiringCompanyDB.ConnectionChannelsClients)
            {
                // channel.Value je objekat tipa ClientCallback

                try
                {
                    channel.Value.SyncData(cData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    forRemove.Add(channel.Key);

                }
            }

            foreach (string key in forRemove)
            {
                Console.WriteLine("removing channel with key {0}", key);
                hiringCompanyDB.ConnectionChannelsClients.Remove(key);
            }
        }

    }
}
