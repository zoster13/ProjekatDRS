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

namespace HiringCompany.Services
{
    /* [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
   ConcurrencyMode = ConcurrencyMode.Reentrant)] */ // mozda perCall, i concurrency na single?

    public class HiringService : IHiringService
    {
        HiringCompanyDB hiringCompanyDb = HiringCompanyDB.Instance();

        public void ResponseForPartnershipRequest(bool accepted, string outsourcingCompName)
        {

            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: HiringService.ResponseForPartnershipRequest(), " +
                                                  "params: bool accepted={0}, string outsorcingCompName={0}", accepted,
                outsourcingCompName));

            string notification = string.Empty;
            if (accepted)
            {
                hiringCompanyDb.PartnerCompaniesAddresses.Remove(outsourcingCompName.Trim());
                //// bacati exc ako ime ne postoji
                hiringCompanyDb.AddNewPartnerCompany(new PartnerCompany(outsourcingCompName));
                notification = "Company <" + outsourcingCompName + "> accepted request for partnership.";
            }
            else
            {
                hiringCompanyDb.ConnectionChannelsCompanies.Remove(outsourcingCompName);
                    // namestiti da ne pada ako posalju pogresno ime..
                notification = "Company <" + outsourcingCompName + "> declined request for partnership.";
            }

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
                notifier.SyncSpecialClients(EmployeeType.CEO, notification);
            }

            messageToLog.AppendLine("finished successfully.");
            Program.Logger.Info(messageToLog);
        }

        public void ResponseForProjectRequest(string outsourcingCompanyName, ProjectCommon p)
        {
            string notification = string.Empty;

            Project proj = new Project();
            try
            {
                using (var access = new AccessDB())
                {
                    proj = access.projects.SingleOrDefault(project => project.Name.Equals(p.Name));
                    if (proj != null)
                    {
                        if (p.IsAcceptedByOutsCompany)
                        {
                            proj.OutsourcingCompany = outsourcingCompanyName;
                            proj.IsAcceptedOutsCompany = true;
                            access.SaveChanges();
                        }
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine(
                        "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
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

            notification = p.IsAcceptedByOutsCompany ? 
                string.Format("Company <" + outsourcingCompanyName + "> accepted request for developing project <" + p.Name + ">.") : 
                string.Format("Company <" + outsourcingCompanyName + "> declined request for developing project <" + p.Name + ">.");
            //if (p.IsAcceptedByOutsCompany)
            //{
                
            //    notification = "Company <" + outsourcingCompanyName + "> accepted request for developing project <" +
            //                   p.Name + ">.";
            //}
            //else
            //{
            //    notification = "Company <" + outsourcingCompanyName + "> declined request for developing project <" +
            //                   p.Name + ">.";
            //}

            using (Notifier notifier = new Notifier())
            {
                if (p.IsAcceptedByOutsCompany) 
                {
                    notifier.SyncAll();
                }              
                notifier.NotifySpecialClient(proj.ProductOwner, notification);
                notifier.NotifySpecialClients(EmployeeType.CEO, notification);
            }
        }


        public void SendUserStoriesToHiringCompany(List<UserStoryCommon> userStories, string projectName)
        {
            List<UserStory> tempUserStories=new List<UserStory>();

            foreach(UserStoryCommon us in userStories)
            {
                UserStory u=new UserStory(us.Title,us.Description,us.AcceptanceCriteria);
                tempUserStories.Add(u);
            }

            using (var access = new AccessDB())
            {
                Project proj = access.projects.SingleOrDefault(project => project.Name.Equals(projectName));
                //mozda se mora raditi onaj include userStories
                if (proj != null)
                {
                    proj.UserStories = tempUserStories;
                    access.SaveChanges();
                }
                
            }
        }
    }
}
