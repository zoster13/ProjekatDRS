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
            
            if (accepted)
            {
                Console.WriteLine("Komunikacija uspostavljena, partnerstvo uspostavljeno");
                // brisanje, namestiti da lockovanje bude u bazi
                hiringCompanyDb.PartnerCompaniesAddresses.Remove(outsourcingCompName.Trim()); // bacati exc ako ime ne postoji
               
                // dodati i mdf bazu 
                hiringCompanyDb.AddNewPartnerCompany(new PartnerCompany(outsourcingCompName));


                // citati iz baze preko linq
                try
                {
                    var access = new AccessDB();
                    var ceos = from x in access.employees
                               where x.Type == EmployeeCommon.EmployeeType.CEO
                               select x;

                    var ceosList = ceos.ToList();

                    foreach (var ceo in ceosList)
                    {
                        
                       

                                 
                        hiringCompanyDb.ConnectionChannelsClients[ceo.Username].Notify("Company <" + outsourcingCompName + "> accepted request for partnership.");
                    }
                    CurrentData cData = new CurrentData();
                    cData.ProjectsForApprovalData = hiringCompanyDb.ProjectsForApproval;
                    cData.AllEmployeesData = hiringCompanyDb.AllEmployees;
                    cData.EmployeesData = hiringCompanyDb.OnlineEmployees;
                    cData.NamesOfCompaniesData = hiringCompanyDb.PartnerCompaniesAddresses.Keys.ToList();
                    cData.ProjectsForSendingData = hiringCompanyDb.ProjectsForSending;
                    cData.CompaniesData = hiringCompanyDb.PartnerCompanies;
                    
                    foreach (IEmployeeServiceCallback call in hiringCompanyDb.ConnectionChannelsClients.Values)
                    {
                        call.SyncData(cData);
                    }

                }
                catch (Exception)
                {
                    
                    throw;
                }

            }
            else
            {
                Console.WriteLine("Zahtev za partnerstvo odbijen");
            }

        }
    }
}
