using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using HiringCompany.DatabaseAccess;
using EmployeeCommon;
using ICommon;
using System.Data.Entity.Validation;
using HiringCompany.Services;
namespace HiringCompany
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class OutsorcingServiceCallback : ICommon.IOutsourcingServiceCallback
    {
        HiringCompanyDB hiringCompanyDb = HiringCompanyDB.Instance();

        public void AskForPartnershipCallback(bool accepted, string outsourcingCompName)
        {
            string notification = string.Empty;

            if (accepted)
            {
                Console.WriteLine("Komunikacija uspostavljena, partnerstvo uspostavljeno");

                notification = "Company <" + outsourcingCompName + "> accepted request for partnership.";

                // brisanje, namestiti da lockovanje bude u bazi
                hiringCompanyDb.PartnerCompaniesAddresses.Remove(outsourcingCompName.Trim()); // bacati exc ako ime ne postoji 
                hiringCompanyDb.AddNewPartnerCompany(new PartnerCompany(outsourcingCompName));


                // citati iz baze preko linq
                try
                {
                    CurrentData cData = new CurrentData();
                    cData.ProjectsForApprovalData = hiringCompanyDb.ProjectsForCeoApproval;
                    cData.AllEmployeesData = hiringCompanyDb.AllEmployees;
                    cData.EmployeesData = hiringCompanyDb.OnlineEmployees;
                    cData.NamesOfCompaniesData = hiringCompanyDb.PartnerCompaniesAddresses.Keys.ToList();
                    cData.ProjectsForSendingData = hiringCompanyDb.ProjectsForSendingToOutsC;
                    cData.CompaniesData = hiringCompanyDb.PartnerCompanies;

                    //foreach (IEmployeeServiceCallback call in hiringCompanyDb.ConnectionChannelsClients.Values)
                    //{
                    //    call.SyncData(cData);
                    //}

                    foreach (var channel in hiringCompanyDb.ConnectionChannelsClients)
                    {
                        try
                        {
                            channel.Value.SyncData(cData);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            hiringCompanyDb.ConnectionChannelsClients.Remove(channel.Key);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            else
            {
                notification = "Company <" + outsourcingCompName + "> declined request for partnership.";

                hiringCompanyDb.ConnectionChannelsCompanies.Remove(outsourcingCompName); // namestiti da ne pada ako posalju pogresno ime..

                Console.WriteLine("Zahtev za partnerstvo odbijen");
            }
            // dodati using
            var access = new AccessDB();
            var ceos = from x in access.employees
                       where x.Type == EmployeeCommon.EmployeeType.CEO
                       select x;

            var ceosList = ceos.ToList();

            foreach (var ceo in ceosList)
            {
                Employee em = hiringCompanyDb.OnlineEmployees.Find(e => e.Username.Equals(ceo.Username)); //treba da saljemo i onima koji nisu online kad se uklkuce,cuvati notifikacije u bazi
                if (em != null)
                {
                    hiringCompanyDb.ConnectionChannelsClients[ceo.Username].Notify(notification);
                }
            }

        }

        public void SendProjectToOutsourcingCompanyCallback(string outsourcingCompanyName, ProjectCommon p) 
        {
            string notification = string.Empty;

            if (p.IsAcceptedByOutsCompany)
            {
                try
                {
                    using (var access = new AccessDB())
                    {
                        Project proj = access.projects.SingleOrDefault(project => project.Name.Equals(p.Name));

                        if (proj != null)
                        {
                            proj.IsAcceptedOutsCompany = true;
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
                notification = "Company <" + outsourcingCompanyName + "> accepted request for developing project <" + p.Name + ">.";

                //treba da se notifikuje CEO,moze i PO
                //svima treba da se uradi Sync

                CurrentData cData = new CurrentData();
                cData.ProjectsForApprovalData = hiringCompanyDb.ProjectsForCeoApproval;
                cData.AllEmployeesData = hiringCompanyDb.AllEmployees;
                cData.EmployeesData = hiringCompanyDb.OnlineEmployees;
                cData.NamesOfCompaniesData = hiringCompanyDb.PartnerCompaniesAddresses.Keys.ToList();
                cData.ProjectsForSendingData = hiringCompanyDb.ProjectsForSendingToOutsC;
                cData.CompaniesData = hiringCompanyDb.PartnerCompanies;

                //foreach (IEmployeeServiceCallback call in hiringCompanyDb.ConnectionChannelsClients.Values)
                //{
                //    call.SyncData(cData);
                //}

                foreach (var channel in hiringCompanyDb.ConnectionChannelsClients)
                {
                    try
                    {
                        channel.Value.SyncData(cData);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        hiringCompanyDb.ConnectionChannelsClients.Remove(channel.Key);
                    }
                }
            }
            else
            {
                notification = "Company <" + outsourcingCompanyName + "> declined request for developing project <" + p.Name + ">.";
                //notifikovati CEO,moze i PO
            }
        }
    }
}
