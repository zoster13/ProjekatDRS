using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon;
using HiringCompany.DatabaseAccess;
using System.ServiceModel;
using EmployeeCommon.Data;

namespace HiringCompany
{
    public class Notifier : IDisposable
    {
        private InternalDatabase internalDatabase = InternalDatabase.Instance();
        private CurrentData cData;

        public Notifier()
        {
            cData = new CurrentData();
        }

        public void SyncAll()
        {

            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: Notifier.SyncAll()");
            Program.Logger.Info(messageToLog);

            PopulateCurrentData();

            List<string> channelsForRemove = new List<string>();

            foreach (var clientChannel in internalDatabase.ConnectionChannelsClients)
            {
                ICommunicationObject cObject = clientChannel.Value as ICommunicationObject;
                if (cObject != null)
                {
                    if (cObject.State == CommunicationState.Opened)
                    {
                        clientChannel.Value.SyncData(cData);
                    }
                    else
                    {
                        channelsForRemove.Add(clientChannel.Key);
                        messageToLog = string.Format("SyncData() failed for client {0}: because clientChannel was in {1} state.", clientChannel.Key, cObject.State.ToString());
                        Program.Logger.Info(messageToLog);
                    }
                }
            }

            foreach (string key in channelsForRemove)
            {
                internalDatabase.ConnectionChannelsClients.Remove(key);
                messageToLog = string.Format("removed channel with key {0}", key);
                Program.Logger.Info(messageToLog);
            }

            messageToLog = "finished successfully";
            Program.Logger.Info(messageToLog);
        }

        public void SyncSpecialClients(EmployeeType type, string notification)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: Notifier.SyncSpecialClients() EmployeeType={0}", type);
            Program.Logger.Info(messageToLog);

            PopulateCurrentData();

            try
            {
                List<string> clients;
                using (var access = new AccessDB())
                {
                    var c = from x in access.Employees
                            where x.Type == type // special type
                            select x.Username;

                    clients = c.ToList();
                }

                List<string> forRemove = new List<string>();

                foreach (var clientUsername in clients)
                {

                    IEmployeeServiceCallback clientChannel;
                    if (internalDatabase.ConnectionChannelsClients.TryGetValue(clientUsername, out clientChannel))
                    {
                        // klijent povezan sa serverom
                        ICommunicationObject cObject = clientChannel as ICommunicationObject;
                        if (cObject != null)
                        {
                            if (cObject.State == CommunicationState.Opened)
                            {
                                clientChannel.SyncData(cData);

                                if (!notification.Equals(string.Empty))
                                {
                                    clientChannel.Notify(notification);
                                }

                            }
                            else
                            {
                                forRemove.Add(clientUsername);
                                messageToLog = string.Format("SyncData() failed for client {0}: because clientChannel was in {1} state.", clientUsername, cObject.State.ToString());
                                Program.Logger.Info(messageToLog);
                            }
                        }
                    }
                    else
                    {                       
                        if (!notification.Equals(""))
                        {
                            using (var access = new AccessDB())
                            {
                                var e = from ems in access.Employees.Include("Notifications")
                                        where ems.Username.Equals(clientUsername)
                                        select ems;

                                var em = e.ToList().FirstOrDefault();
                                em.Notifications.Add(new Notification(DateTime.Now.ToShortDateString(), notification));
                                access.SaveChanges();
                                messageToLog = "Notification added to .mdf.";
                                Program.Logger.Info(messageToLog);
                            }
                        }

                    }
                }

                foreach (string key in forRemove)
                {
                    internalDatabase.ConnectionChannelsClients.Remove(key);
                    messageToLog = string.Format("removed channel with key {0}", key);
                    Program.Logger.Info(messageToLog);
                }

                messageToLog = "finished successfully";
                Program.Logger.Info(messageToLog);

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Program.Logger.Info(messageToLog);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                        Program.Logger.Info(messageToLog);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
            messageToLog = "finished successfully";
            Program.Logger.Info(messageToLog);
        }

        public void NotifySpecialClients(EmployeeType type, string notification)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: Notifier.NotifySpecialClients(), EmployeeType={0}", type);
            Program.Logger.Info(messageToLog);

            PopulateCurrentData();

            try
            {
                List<string> clients;
                using (var access = new AccessDB())
                {
                    var c = from x in access.Employees
                            where x.Type == type // special type
                            select x.Username;

                    clients = c.ToList();

                }

                List<string> forRemove = new List<string>();

                foreach (var clientUsername in clients)
                {

                    IEmployeeServiceCallback clientChannel;
                    if (internalDatabase.ConnectionChannelsClients.TryGetValue(clientUsername, out clientChannel))
                    {
                        // klijent povezan sa serverom
                        ICommunicationObject cObject = clientChannel as ICommunicationObject;
                        if (cObject != null)
                        {
                            if (cObject.State == CommunicationState.Opened)
                            {
                                clientChannel.Notify(notification);
                            }
                            else
                            {
                                forRemove.Add(clientUsername);
                                messageToLog = string.Format("NotifySpecialClients() failed for client {0}: because clientChannel was in {1} state.", clientUsername, cObject.State.ToString());
                                Program.Logger.Info(messageToLog);
                            }
                        }
                    }
                    else
                    { 
                        if (!notification.Equals(""))
                        {
                            using (var access = new AccessDB())
                            {
                                var e = from ems in access.Employees.Include("Notifications")
                                        where ems.Username.Equals(clientUsername)
                                        select ems;

                                var em = e.ToList().FirstOrDefault();
                                em.Notifications.Add(new Notification(DateTime.Now.ToShortDateString(), notification));
                                access.SaveChanges();
                                messageToLog = "Notification added to .mdf.";
                                Program.Logger.Info(messageToLog);
                            }
                        }
                    }
                }

                foreach (string key in forRemove)
                {
                    internalDatabase.ConnectionChannelsClients.Remove(key);
                    messageToLog = string.Format("removed channel with key {0}", key);
                    Program.Logger.Info(messageToLog);
                }

                messageToLog = "finished successfully";
                Program.Logger.Info(messageToLog);

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog = string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            messageToLog = "finished successfully";
            Program.Logger.Info(messageToLog);


        }

        public void NotifySpecialClient(string clientUsername, string notification)
        {
            string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: Notifier.SyncSpecialClient() clientUsername={0}", clientUsername);
            Program.Logger.Info(messageToLog);

            IEmployeeServiceCallback clientChannel;
            ICommunicationObject cObject;
            if (internalDatabase.ConnectionChannelsClients.TryGetValue(clientUsername, out clientChannel))
            {
                // klijent povezan sa serverom
                cObject = clientChannel as ICommunicationObject;
                if (cObject != null)
                {
                    if (cObject.State == CommunicationState.Opened)
                    {
                        clientChannel.Notify(notification);
                    }
                    else
                    {
                        internalDatabase.ConnectionChannelsClients.Remove(clientUsername);
                        messageToLog = string.Format("NotifySpecialClient() failed for client {0}: because clientChannel was in {1} state.", clientUsername, cObject.State.ToString());
                        Program.Logger.Info(messageToLog);
                    }
                }
            }
            else
            {
                messageToLog = string.Format("Client is not available currently.");
                Program.Logger.Info(messageToLog);
                if (!notification.Equals(""))
                {
                    using (var access = new AccessDB())
                    {
                        var e = from ems in access.Employees.Include("Notifications")
                                where ems.Username.Equals(clientUsername)
                                select ems;

                        var em = e.ToList().FirstOrDefault();
                        em.Notifications.Add(new Notification(DateTime.Now.ToShortDateString(), notification));
                        access.SaveChanges();
                        messageToLog = "Notification added to .mdf.";
                        Program.Logger.Info(messageToLog);
                    }
                }
            }

        }

        private void PopulateCurrentData()
        {
            cData.EmployeesData = internalDatabase.OnlineEmployees;
            cData.AllEmployeesData = HiringCompanyDB.Instance.AllEmployees();

            cData.ProjectsForApprovalData = HiringCompanyDB.Instance.ProjectsForCeoApproval();
            cData.ProjectsForSendingData = HiringCompanyDB.Instance.ProjectsForSendingToOutsC();
            cData.ProjectsInDevelopmentData = HiringCompanyDB.Instance.ProjectsInDevelopment();

            cData.CompaniesData = HiringCompanyDB.Instance.PartnerCompanies();
            cData.NamesOfCompaniesData = internalDatabase.PossiblePartnersAddresses.Keys.ToList();
        }

        public void Dispose()
        {
        }
    }
}
