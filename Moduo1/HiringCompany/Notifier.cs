using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon;
using HiringCompany.DatabaseAccess;
using System.ServiceModel;

namespace HiringCompany
{
    // mozda ovo treba kao neki singletone implementirati?
    // obavezno treba da omogucimo da se notifikacije cuvaju u bazi!!!

    public class Notifier : IDisposable
    {

        HiringCompanyDB hiringCompanyDB = HiringCompanyDB.Instance();
        private CurrentData cData;

        public Notifier()
        {
            cData = new CurrentData();
        }

        public void SyncAll()
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: Notifier.SyncAll()"));

            PopulateCurrentData();

            List<string> channelsForRemove = new List<string>();

            foreach (var clientChannel in hiringCompanyDB.ConnectionChannelsClients)
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
                        messageToLog.AppendLine(string.Format("SyncData() failed for client {0}: because clientChannel was in {1} state.", clientChannel.Key, cObject.State.ToString()));
                    }
                }
            }

            foreach (string key in channelsForRemove)
            {
                hiringCompanyDB.ConnectionChannelsClients.Remove(key);
                messageToLog.AppendLine(string.Format("removed channel with key {0}", key));
            }

            messageToLog.AppendLine("finished successfully");
            Program.Logger.Info(messageToLog);
        }

        public void SyncSpecialClients(EmployeeType type, string notification)
        {

            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: Notifier.SyncSpecialClients()")); // dodati params, srediti loger i to

            PopulateCurrentData();

            try
            {
                List<string> clients;
                using (var access = new AccessDB())
                {
                    var c = from x in access.employees
                            where x.Type == type // special type
                            select (x.Username);

                    clients = c.ToList();
                }

                List<string> forRemove = new List<string>();

                foreach (var clientUsername in clients)
                {

                   IEmployeeServiceCallback clientChannel;
                    if (hiringCompanyDB.ConnectionChannelsClients.TryGetValue(clientUsername, out clientChannel))
                    // klijent povezan sa serverom
                    {
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
                                messageToLog.AppendLine(string.Format("SyncData() failed for client {0}: because clientChannel was in {1} state.", clientUsername, cObject.State.ToString()));
                            }
                        }
                    }
                    else
                    { // treba cuvati notifikaciju u bazi

                    }
                }

                foreach (string key in forRemove)
                {
                    hiringCompanyDB.ConnectionChannelsClients.Remove(key);
                    messageToLog.AppendLine(string.Format("removed channel with key {0}", key));
                }

                messageToLog.AppendLine("finished successfully");
                Program.Logger.Info(messageToLog);

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
            catch (Exception e) // videti sta je ovde bilo greska
            {
                Console.WriteLine(e);

            }


            messageToLog.AppendLine("finished successfully");
            Program.Logger.Info(messageToLog);
        }

        public void NotifySpecialClients(EmployeeType type, string notification)
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: Notifier.NotifySpecialClients()")); // dodati params, srediti loger i to

            PopulateCurrentData();

            try
            {
                List<string> clients;
                using (var access = new AccessDB())
                {
                    var c = from x in access.employees
                            where x.Type == type // special type
                            select (x.Username);

                    clients = c.ToList();
                }

                List<string> forRemove = new List<string>();

                foreach (var clientUsername in clients)
                {

                    IEmployeeServiceCallback clientChannel;
                    if (hiringCompanyDB.ConnectionChannelsClients.TryGetValue(clientUsername, out clientChannel))
                    // klijent povezan sa serverom
                    {
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
                                messageToLog.AppendLine(string.Format("SyncData() failed for client {0}: because clientChannel was in {1} state.", clientUsername, cObject.State.ToString()));
                            }
                        }
                    }
                    else
                    { // treba cuvati notifikaciju u bazi

                    }
                }

                foreach (string key in forRemove)
                {
                    hiringCompanyDB.ConnectionChannelsClients.Remove(key);
                    messageToLog.AppendLine(string.Format("removed channel with key {0}", key));
                }

                messageToLog.AppendLine("finished successfully");
                Program.Logger.Info(messageToLog);

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
            catch (Exception e) // videti sta je ovde bilo greska
            {
                Console.WriteLine(e);

            }


            messageToLog.AppendLine("finished successfully");
            Program.Logger.Info(messageToLog);


        }
        public void NotifySpecialClient(string clientUsername, string notification)
        {
            // probati da izvuces kanal, ako ga ima poslati notifiakciju, ako je nema, sacuvati je u bazi i poslati je kad se korisnik prijavi..
            // kas se korisniku posalju notifikacije, onda mozemo da ih izbrisemo iz baze?

            IEmployeeServiceCallback clientChannel;
            if (hiringCompanyDB.ConnectionChannelsClients.TryGetValue(clientUsername, out clientChannel))
            // klijent povezan sa serverom
            {
                ICommunicationObject cObject = clientChannel as ICommunicationObject;
                if (cObject != null)
                {
                    if (cObject.State == CommunicationState.Opened)
                    {
                        clientChannel.Notify(notification);
                    }
                    else
                    {
                        hiringCompanyDB.ConnectionChannelsClients.Remove(clientUsername);
                    }
                }
            }
            else // cuvaj u bazi, napraviti metodu u ovom notifieru koja se poziva iz sign in, koaj ce citati sve notifikacije iz baze i slati klijentu
            {

            }

        }

        private void PopulateCurrentData()
        {
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;

            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForCeoApproval;
            cData.ProjectsForSendingData = hiringCompanyDB.ProjectsForSendingToOutsC;
            cData.ProjectsInDevelopmentData = hiringCompanyDB.ProjectsInDevelopment;

            cData.CompaniesData = hiringCompanyDB.PartnerCompanies;
            cData.NamesOfCompaniesData = hiringCompanyDB.PartnerCompaniesAddresses.Keys.ToList();
        }

        public void Dispose() // nemam pojma kako se ovaj notifier ponasa sa visi niti, ali ne znam ni baza kako se ponasa bla bla
        {
            // throw new NotImplementedException();
        }
    }
}
