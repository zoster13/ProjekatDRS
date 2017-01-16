using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon;
using HiringCompany.DatabaseAccess;
using ICommon;
using EmployeeCommon.Data;

namespace HiringCompany.Services
{
    public class HiringService : IHiringService
    {
        private InternalDatabase internalDatabase = InternalDatabase.Instance();

        public void ResponseForPartnershipRequest(bool accepted, string outsourcingCompName)
        {

           string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: HiringService.ResponseForPartnershipRequest(), " +
                                                  "params: bool accepted={0}, string outsorcingCompName={0}", accepted, outsourcingCompName);
            Program.Logger.Info(messageToLog);

            string notification = string.Empty;
            if (accepted)
            {
                internalDatabase.PartnerCompaniesAddresses.Add(outsourcingCompName.Trim(), internalDatabase.PossiblePartnersAddresses[outsourcingCompName]);
                internalDatabase.PossiblePartnersAddresses.Remove(outsourcingCompName.Trim());

                HiringCompanyDB.Instance.AddNewPartnerCompany(new PartnerCompany(outsourcingCompName)); // adding to db
                notification = "Company <" + outsourcingCompName + "> accepted request for partnership.";
            }
            else
            {             
                notification = "Company <" + outsourcingCompName + "> declined request for partnership.";
            }

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
                notifier.SyncSpecialClients(EmployeeType.CEO, notification);
            }
        }

        public void ResponseForProjectRequest(string outsourcingCompanyName, ProjectCommon p)
        {
           string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: HiringService.ResponseForProjectRequest(), " +
                                                  "params: string outsorcingCompName={0}, Project.Name={1}", outsourcingCompanyName, p.Name);
            Program.Logger.Info(messageToLog);

            HiringCompanyDB.Instance.ResponseForProjectRequestFieldsChange(outsourcingCompanyName, p);
        }

        public void SendUserStoriesToHiringCompany(List<UserStoryCommon> userStories, string projectName)
        {
           string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: HiringService.ResponseForProjectRequest(), " +
                                                  "params: Project.Name={0}, userStories.Count={1} ", projectName, userStories.Count);


            Program.Logger.Info(messageToLog);

            List<UserStory> tempUserStories = new List<UserStory>();

            foreach (UserStoryCommon us in userStories)
            {
                UserStory u = new UserStory(us.Title, us.Description, us.AcceptanceCriteria);
                tempUserStories.Add(u);
            }

            HiringCompanyDB.Instance.SendUserStoriesToHiringCompanyFieldsChange(tempUserStories, projectName);
        }


        public void SendClosedUserStory(string projectName, string title)
        {
           string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: HiringService.SendClosedUserStory(), " +
                                                  "params: Project.Name={0}, UserStory.Title={1} ", projectName, title);
            Program.Logger.Info(messageToLog);

            HiringCompanyDB.Instance.SendClosedUserStoryFieldChange(projectName, title);
        }
    }
}
