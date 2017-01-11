using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using HiringCompany.DatabaseAccess;
using EmployeeCommon;
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
                Employee em = hiringCompanyDb.OnlineEmployees.Find(e => e.Username.Equals(ceo.Username));
                if (em != null)
                {
                    hiringCompanyDb.ConnectionChannelsClients[ceo.Username].Notify(notification);
                }
            }

        }
    }
}
