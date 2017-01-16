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
using EmployeeCommon.Data;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace HiringCompany.Services
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
      ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class EmployeeService : IEmployeeService
    {

        //private IHiringCompanyDB hiringCompanyDB = HiringCompanyDB.Instance();
        private InternalDatabase internalDatabase = InternalDatabase.Instance();
        private OutsorcingCompProxy outsorcingProxy;

        private System.Timers.Timer lateOnJobTimer = new System.Timers.Timer();

        private System.Timers.Timer passwordExpired = new System.Timers.Timer();

        private System.Timers.Timer userStoriesDeadlineWarning = new System.Timers.Timer();

        public EmployeeService()
        {
            lateOnJobTimer.Elapsed += new ElapsedEventHandler(NotifyOnLate);
            lateOnJobTimer.Interval = 900000; // to je 15min
            //lateOnJobTimer.Enabled = true;

            passwordExpired.Elapsed += new ElapsedEventHandler(NotifyPasswordExpired);
            passwordExpired.Interval = 30000; // 30s, ostavila samo za proveru,staviti ovo na neki mnogo veci broj
           // passwordExpired.Enabled = true;

            userStoriesDeadlineWarning.Elapsed += new ElapsedEventHandler(NotifyUserStoriesDeadline);
            userStoriesDeadlineWarning.Interval = 30000; // samo za proveru,treba staviti da se jednom dnevno proverava
            //userStoriesDeadlineWarning.Enabled = true;
        }

        // slanje maila onima koji nisu online, srediti ovu metodu body i content od maila...
        // i srediti raspored kad se ovo ukljucuje i iskljucuje i slicno
        /*ideja: kad se pokrene servis izvuku se sati dolazaka svih klijenata,
         *  i izracuna se sat najkasnijeg dolaska, i kad prodje taj sat vise nema potrebe da se vrti onaj timer*/
        public void NotifyOnLate(object sender, ElapsedEventArgs e)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.NotifyOnLate()");

            string _senderEmailAddress = "mzftn123fakultet@gmail.com"; // i ovo cuvati u nekom fajlu
            string _senderPassword = "miljanazvezdana123";
            Console.WriteLine("alarm...");
            foreach (Employee em in HiringCompanyDB.Instance.AllEmployees())
            {
                if (!internalDatabase.OnlineEmployees.Contains(em))
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

            messageToLog = "finished successfully.";
            Program.Logger.Info(messageToLog);
        }

        public void NotifyPasswordExpired(object sender, ElapsedEventArgs e)
        {
            string notification = "Your password has expired, you have to change it.";
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.NotifyPasswordExpired()");

            Console.WriteLine("alarm2...");
            foreach (Employee em in HiringCompanyDB.Instance.AllEmployees())
            {
                DateTime current = DateTime.Now;

                if(em.DatePasswordChanged.AddMonths(6) < current)
                {
                    using (Notifier notifier = new Notifier())
                    {
                        notifier.NotifySpecialClient(em.Username, notification);
                    }
                }
            }

            messageToLog = "finished successfully.";
            Program.Logger.Info(messageToLog);
        }

        public void NotifyUserStoriesDeadline(object sender, ElapsedEventArgs e)
        {
            string notification = string.Empty;
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.NotifyUserStoriesDeadline()");

            Console.WriteLine("alarm3...");
            foreach (Project p in HiringCompanyDB.Instance.ProjectsInDevelopment())
            {
                DateTime current = DateTime.Now;
                if (!p.IsClosed && p.UserStories.Count!=0 && p.IsAcceptedCEO && p.IsAcceptedOutsCompany)
                {
                    if(current.AddDays(10)>p.Deadline)
                    {
                        List<UserStory> us = p.UserStories.FindAll(u => u.IsClosed == false);
                        if (((double)p.UserStories.Count / 5) <= us.Count)
                        {
                            using (Notifier notifier = new Notifier())
                            {
                                notification = string.Format("More than 20% of user stories for project <{0}> is not closed.", p.Name);
                                notifier.NotifySpecialClient(p.ScrumMaster, notification);
                            }
                        }                       
                    }
                }
            }

            messageToLog = "finished successfully.";
            Program.Logger.Info(messageToLog);
        }

        public bool SignIn(string username, string password)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.SignIn(), " +
                                              "params: string username={0}, string password={0}", username, password);
            Program.Logger.Info(messageToLog);

            Employee employee = HiringCompanyDB.Instance.GetEmployee(username);

            if (employee != null && password.Equals(employee.Password))
            {
                if (OperationContext.Current != null)
                {
                    IEmployeeServiceCallback callbackClient = OperationContext.Current.GetCallbackChannel<IEmployeeServiceCallback>();

                    ICommunicationObject cObject = callbackClient as ICommunicationObject;
                    if (cObject != null)
                    {
                        IEmployeeServiceCallback outCallback;
                        if (!internalDatabase.ConnectionChannelsClients.TryGetValue(username, out outCallback))
                        {
                            internalDatabase.ConnectionChannelsClients.Add(username, callbackClient);

                            lock (internalDatabase.OnlineEmployees_lock)
                            {
                                internalDatabase.OnlineEmployees.Add(employee);
                            }

                            messageToLog = "finished successfully.";
                            Program.Logger.Info(messageToLog);
                            using (Notifier notifier = new Notifier())
                            {
                                notifier.SyncAll();
                            }

                            Employee toNotifEmployee = new Employee();
                            toNotifEmployee = HiringCompanyDB.Instance.GetEmployee(username);

                            using (Notifier notifier = new Notifier())
                            {
                                foreach (var notif in toNotifEmployee.Notifications)
                                {
                                    notifier.NotifySpecialClient(username, string.Format("{0}: {1}", notif.Timestamp, notif.Content));
                                }
                            }

                            HiringCompanyDB.Instance.ClearEmployeeNotifs(username);
                        }
                        else
                        {
                            messageToLog = "Channel associated with username already exists.";
                            Program.Logger.Info(messageToLog);
                        }
                    }
                }
            }
            else
            {
                messageToLog = "Employee equals null, or password was wrong. returned false";
                Program.Logger.Info(messageToLog);
                return false;
            }

            messageToLog = "returned true";
            Program.Logger.Info(messageToLog);
            return true;
        }

        public void SignOut(string username)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.SignOut(), " +
                                              "params: string username={0}", username);
            Program.Logger.Info(messageToLog);

            lock (internalDatabase.OnlineEmployees_lock)
            {
                Employee em = internalDatabase.OnlineEmployees.Find(e => e.Username.Equals(username));
                if (em != null)
                {
                    internalDatabase.OnlineEmployees.Remove(em);
                    internalDatabase.ConnectionChannelsClients.Remove(username);
                    messageToLog = "finished successfully.";
                    Program.Logger.Info(messageToLog);
                }
                else
                {
                    messageToLog = "employee does not exist.";
                    Program.Logger.Info(messageToLog);
                }
            }

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
            }

            Program.Logger.Info(messageToLog);
        }

        public void ChangeEmployeeData(string username, string name, string surname, string email, string password)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.ChangeEmployeeData(), " +
                                              "params: string username={0}, string name={1}," +
                                                  " string surname={2}, string email={3}, string password={4}",
                                                  username, name, surname, email, password);
            Program.Logger.Info(messageToLog);
            HiringCompanyDB.Instance.EditEmployeeData(username, name, surname, email, password);
            internalDatabase.EditOnlineEmployeeData(username, name, surname, email, password);

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
            }

            Program.Logger.Info(messageToLog);
        }

        public void SetWorkingHours(string username, int beginH, int beginM, int endH, int endM)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.SetWorkingHours(), " +
                                              "params: string username={0}, int beginH={1}, int beginM={2}," +
                                                  "int endH={3}, int endM={4}", username, beginH, beginM, endH, endM);

            HiringCompanyDB.Instance.EditWorkingHours(username, beginH, beginM, endH, endM);
            internalDatabase.EditWorkingHoursForOnlineEm(username, beginH, beginM, endH, endM);

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
            }

            messageToLog = "returned.";
            Program.Logger.Info(messageToLog);
        }

        public void AskForPartnership(string outsorcingCompanyName)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.AskForPartnership(), " +
                                              "params: string outsorcingCompanuName={0}", outsorcingCompanyName);
            Program.Logger.Info(messageToLog);

            string outsorcingSvcEndpoint = string.Format("net.tcp://{0}/OutsourcingService", internalDatabase.PossiblePartnersAddresses[outsorcingCompanyName]);

            NetTcpBinding binding = new NetTcpBinding();
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(outsorcingSvcEndpoint));

            using (outsorcingProxy = new OutsorcingCompProxy(binding, endpointAddress))
            {
                outsorcingProxy.AskForPartnership(internalDatabase.CompanyName);
                messageToLog = "called method for sending partnership request on OutsorcingCompProxy successfully.";
            }

            Program.Logger.Info(messageToLog);
        }

        public bool AddNewEmployee(Employee em)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: bool EmployeeService.AddNewEmployee(), Employee.Username={0}", em.Username);
            Program.Logger.Info(messageToLog);

            bool retVal = HiringCompanyDB.Instance.AddNewEmployee(em);

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
            }

            messageToLog = string.Format("returned {0}.", retVal);
            Program.Logger.Info(messageToLog);

            return retVal;
        }

        public void ChangeEmployeeType(string username, EmployeeType type)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.ChangeEmployeeType(), " +
                                              "params: string username={0}, string EmployeeType={0}", username, type.ToString());
            Program.Logger.Info(messageToLog);

            HiringCompanyDB.Instance.EditEmployeeType(username, type);
            internalDatabase.EditOnlineEmployeeType(username, type);

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
            }
            Program.Logger.Info(messageToLog);
        }

        public void SendApprovedUserStories(string projectName, List<UserStory> userStories)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.SendApprovedUserStories(), " +
                                              "params: string projectName={0};  userStories.Count={1}", projectName, userStories.Count);
            Program.Logger.Info(messageToLog);

            Project proj=HiringCompanyDB.Instance.SendApprovedUserStoriesFieldChange(projectName, userStories);

            //sklonicemo ove komentare sutra,kad proverimo da li radi u komunikaciji sa drugim serverom
            //Project proj = new Project();
            //try
            //{
            //    using (var access = new AccessDB())
            //    {
            //        proj = access.Projects.Include("UserStories").SingleOrDefault(p => p.Name.Equals(projectName));

            //        if (proj != null)
            //        {
            //            if (proj.UserStories.Count == userStories.Count)
            //            {
            //                for (int i=0; i < proj.UserStories.Count; i++)
            //                {
            //                    proj.UserStories[i].IsApprovedByPO = userStories[i].IsApprovedByPO;
            //                }
            //            }
            //            else
            //            {
            //                messageToLog = "Unsuccessful idea! :( ";
            //                Program.Logger.Info(messageToLog);
            //            }

            //            access.SaveChanges();

            //            messageToLog = "changed user stories data in .mdf database.";
            //            Program.Logger.Info(messageToLog);
            //        }
            //    }
            //}
            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        Program.Logger.Info(messageToLog);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
            //                ve.PropertyName,
            //                eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
            //                ve.ErrorMessage);
            //            Program.Logger.Info(messageToLog);
            //        }
            //    }
            //}

            string outsorcingSvcEndpoint = string.Format("net.tcp://{0}/OutsourcingService", internalDatabase.PartnerCompaniesAddresses[proj.OutsourcingCompany]);

            NetTcpBinding binding = new NetTcpBinding();
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(outsorcingSvcEndpoint));

            List<UserStoryCommon> userStoriesForSend = new List<UserStoryCommon>();
            foreach (var us in userStories)
            {
                userStoriesForSend.Add(new UserStoryCommon(us.Title, us.Description, us.AcceptanceCriteria, us.IsApprovedByPO));
            }

            using (outsorcingProxy = new OutsorcingCompProxy(binding, endpointAddress))
            {
                outsorcingProxy.SendEvaluatedUserstoriesToOutsourcingCompany(userStoriesForSend, projectName);
                messageToLog = "called method for sending approved user stories on OutsorcingCompProxy successfully.";
                Program.Logger.Info(messageToLog);
            }

            HiringCompanyDB.Instance.RemoveDeclinedUserStoriesFromDB(projectName);

            //sklonicemo ove komentare sutra,kad proverimo da li radi u komunikaciji sa drugim serverom
            //try
            //{
            //    // ovo treba da radimo u bazi 
            //    using (var access = new AccessDB())
            //    {
            //        proj = access.Projects.Include("UserStories").SingleOrDefault(p => p.Name.Equals(projectName));
            //        proj.UserStories.RemoveAll(us => us.IsApprovedByPO == false);
            //        access.SaveChanges();

            //        messageToLog = "Removed user stories that were not approved in .mdf database.";
            //        Program.Logger.Info(messageToLog);
            //    }
            //}
            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        Program.Logger.Info(messageToLog);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
            //                ve.PropertyName,
            //                eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
            //                ve.ErrorMessage);
            //            Program.Logger.Info(messageToLog);
            //        }
            //    }
            //}

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
            }

        }

        public void CreateNewProject(Project p)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.CreateNewProject(), Project.Name={0}", p.Name);
            Program.Logger.Info(messageToLog);

            HiringCompanyDB.Instance.AddNewProject(p);

            string notification = string.Format("Project <{0}> is waiting for approval. Description: {1}", p.Name, p.Description);

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncSpecialClients(EmployeeType.CEO, notification);
            }

            messageToLog = "finished successfully.";
            Program.Logger.Info(messageToLog);
        }

        public void ProjectApprovedByCeo(Project p)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.ProjectApprovedByCeo(), Project.Name={0}", p.Name);
            Program.Logger.Info(messageToLog);
            HiringCompanyDB.Instance.ProjectApprovedCEOFieldChange(p);

            string notification = string.Format("Project {0} is approved. Description: {1}", p.Name, p.Description);
            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
                notifier.NotifySpecialClient(p.ProductOwner, notification);
            }

            messageToLog = "finished successfully.";
            Program.Logger.Info(messageToLog);
        }

        public void SendProject(string outscCompany, Project p)
        {

            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.SendProject(), " +
                                              "params: string outscCompany={0}, Project.Name={1}", outscCompany, p.Name);

            Program.Logger.Info(messageToLog);

            ProjectCommon proj = new ProjectCommon(p.Name, p.Description, p.StartDate, p.Deadline);

            string outsorcingSvcEndpoint = string.Format("net.tcp://{0}/OutsourcingService", internalDatabase.PartnerCompaniesAddresses[outscCompany]);

            NetTcpBinding binding = new NetTcpBinding();
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(outsorcingSvcEndpoint));


            using (outsorcingProxy = new OutsorcingCompProxy(binding, endpointAddress))
            {
                outsorcingProxy.SendProjectToOutsourcingCompany(internalDatabase.CompanyName, proj);
                messageToLog = "called method for sending project on OutsorcingCompProxy successfully.";
                Program.Logger.Info(messageToLog);
            }

            Program.Logger.Info(messageToLog);
        }


        public void CloseProject(string projectName)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: EmployeeService.CloseProject(), " +
                                                  "params: Project.Name={0}", projectName);

            Program.Logger.Info(messageToLog);

            HiringCompanyDB.Instance.CloseProjectFieldChange(projectName);

            string notification = string.Format("Project <{0}> is closed.", projectName);

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
                notifier.NotifySpecialClients(EmployeeType.CEO, notification);
            }
        }
    }
}
