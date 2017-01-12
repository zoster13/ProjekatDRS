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
        // i srediti raspored kad se ovo ukljucuje i iskljucuje i slicno
        private void NotifyOnLate( object sender, ElapsedEventArgs e )
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: EmployeeService.NotifyOnLate()"));

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

            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);
        }

        public bool SignIn( string username, string password )
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: EmployeeService.SignIn(), " +
                                              "params: string username={0}, string password={0}", username, password));

            Employee employee = hiringCompanyDB.GetEmployee(username);

            if (employee != null && password.Equals(employee.Password))
            {
                IEmployeeServiceCallback callbackClient = OperationContext.Current.GetCallbackChannel<IEmployeeServiceCallback>();

                // Console.WriteLine(( (IClientChannel)callbackClient ).State.ToString());
                ICommunicationObject cObject = callbackClient as ICommunicationObject;
                if (cObject != null)
                {
                    IEmployeeServiceCallback outCallback;
                    if (!hiringCompanyDB.ConnectionChannelsClients.TryGetValue(username, out outCallback))
                    {
                        hiringCompanyDB.ConnectionChannelsClients.Add(username, callbackClient);

                        lock (hiringCompanyDB.OnlineEmployees_lock) 
                        {
                            hiringCompanyDB.OnlineEmployees.Add(employee);
                        }

                        using (Notifier notifier = new Notifier())
                        {
                            notifier.SyncAll();
                        }

                    }
                    else
                    {
                        messageToLog.AppendLine("Channel associated with username already exists.");
                        // taj kanal vec postoji, tj. nije izbrisan iz ConnectionChannelsClients,
                        // iako klijent svaki put kad se loguje pravi novi kanal, nesto nije u redu...
                        // mozda ovde da uradimo remove starog kanala, i sacuvamo novi?
                        // i da proverimo da li je vec dodati u online employees 
                    }
                }

            }
            else
            {
                messageToLog.AppendLine("Employee equals null, or password was wrong.");
                return false;
            }

            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);
            return true;
        }

        public void SignOut( string username )
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: EmployeeService.SignOut(), " +
                                              "params: string username={0}", username));

        
            lock (hiringCompanyDB.OnlineEmployees_lock)
            {
                Employee em = hiringCompanyDB.OnlineEmployees.Find(e => e.Username.Equals(username));
                if (em != null)
                {
                    hiringCompanyDB.OnlineEmployees.Remove(em);
                    hiringCompanyDB.ConnectionChannelsClients.Remove(username);
                }
                else
                {
                    messageToLog.AppendLine("employee does not exist.");
                }
            }

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
            }

            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);
        }

        public void ChangeEmployeeData( string username, string name, string surname, string email, string password )
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: EmployeeService.ChangeEmployeeData(), " +
                                              "params: string username={0}, string name={1}," +
                                                  " string surname={2}, string email={3}, string password={4}",
                                                  username, name, surname, email, password));

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
                    messageToLog.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog.AppendLine(string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                    }
                }
            }

            lock (hiringCompanyDB.OnlineEmployees_lock)
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

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
            }


            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);
        }

        public void SetWorkingHours( string username, int beginH, int beginM, int endH, int endM )
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: EmployeeService.SetWorkingHours(), " +
                                              "params: string username={0}, int beginH={1}, int beginM={2}," +
                                                  "int endH={3}, int endM={4}", username, beginH, beginM, endH, endM));

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
                    messageToLog.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog.AppendLine(string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                    }
                }
            }

            lock (hiringCompanyDB.OnlineEmployees_lock)
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

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
            }

            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);
        }

        public void AskForPartnership( string outsorcingCompanyName )
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: EmployeeService.AskForPartnership(), " +
                                              "params: string outsorcingCompanuName={0}", outsorcingCompanyName));

            //ovde namestiti da iz baze-mape, ili fajla iscitamo adresu te kompanije

            string outsorcingSvcEndpoint = string.Format("net.tcp://{0}/OutsourcingService", hiringCompanyDB.PartnerCompaniesAddresses[outsorcingCompanyName]);

            NetTcpBinding binding = new NetTcpBinding();
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(outsorcingSvcEndpoint));


            // izbrisati iz liste, i dodati ako negde nesto treba
            using (outsorcingProxy = new OutsorcingCompProxy(binding, endpointAddress))
            {
                outsorcingProxy.AskForPartnership(hiringCompanyDB.CompanyName);   // i onda kasnije kad pozivamo neke motede uvek proveravmao da li smo partneri
            }

            // sacuvati proxy, namestiti lockovabhe.. ma ne treba da ga cuvamo
            //hiringCompanyDB.ConnectionChannelsCompanies.Add(outsorcingCompanyName, outsorcingProxy);

            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);
        }

        public bool AddNewEmployee( Employee em )
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: EmployeeService.AddNewEmployee()"));

            bool retVal = hiringCompanyDB.AddNewEmployee(em);

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
            }

            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);

            return retVal;
        }

        public void ChangeEmployeeType( string username, EmployeeType type )
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: EmployeeService.ChangeEmployeeType(), " +
                                              "params: string username={0}, string EmployeeType={0}", username, type.ToString()));

            try
            {
                using (var access = new AccessDB())
                {
                    Employee em = access.employees.SingleOrDefault(e => e.Username.Equals(username));
                    if (em != null)
                    {
                        em.Type = type;
                        access.SaveChanges();
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog.AppendLine(string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                    }
                }
            }

            lock (hiringCompanyDB.OnlineEmployees_lock)
            {
                Employee em = hiringCompanyDB.OnlineEmployees.Find(e => e.Username.Equals(username));
                if (em != null)
                {
                    em.Type = type;
                }
            }

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
            }

            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);
        }

        public void SendApprovedUserStories(string projectName,List<UserStory> userStories)
        {
            //promeniti u bazi polje isApproved za sve odobrene userStories
            //pozvati outsComp metodu i proslediti listu userStories
            
        }

        public void CreateNewProject( Project p )
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: EmployeeService.CreateNewProject(), "));

            hiringCompanyDB.AddNewProject(p); // onaj neki lock

            string notification = ( string.Format("Project {0} is waiting for approval.", p.Name));

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncSpecialClients(EmployeeType.CEO, notification);
            }

            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);
        }

        public void ProjectApprovedByCeo( Project p )
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: EmployeeService.ProjectApprovedByCeo()"));

            try
            {
                using (var access = new AccessDB())
                {
                    var project = from proj in access.projects
                                  where proj.Name.Equals(p.Name)
                                  select proj;

                    var pr = project.ToList().FirstOrDefault();
                    pr.IsAcceptedCEO = true;
                    access.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog.AppendLine(string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                    }
                }
            }

            string notification = string.Format("Project {0} is approved.", p.Name);
            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
                notifier.NotifySpecialClient(p.ProductOwner, notification);
            }

            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);
        }

        public void SendProject( string outscCompany, Project p )
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: EmployeeService.SendProject(), " +
                                              "params: string outscCompany={0}", outscCompany));


            ProjectCommon proj = new ProjectCommon(p.Name, p.Description, p.StartDate, p.Deadline);

            // ovde prvo treba da proverimo da li smo partneri!!!!!!!!!!!!!!!!!!

            //ovde namestiti da iz baze-mape, ili fajla iscitamo adresu te kompanije

            string outsorcingSvcEndpoint = string.Format("net.tcp://{0}/OutsourcingService", hiringCompanyDB.PartnerCompaniesAddresses[outscCompany]);

            NetTcpBinding binding = new NetTcpBinding();
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(outsorcingSvcEndpoint));


            using (outsorcingProxy = new OutsorcingCompProxy(binding, endpointAddress))
            {
                outsorcingProxy.SendProjectToOutsourcingCompany(hiringCompanyDB.CompanyName, proj);  
            }
        
            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);
        }
    }
}
