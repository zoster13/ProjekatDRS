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
    /* [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
   ConcurrencyMode = ConcurrencyMode.Reentrant)]  // nema smisla ovo reentrant sad*/
    // mozda perCall, i concurrency na single?
    // pitati njih na koji nacin oni pozivaju nas servis, da li per call?
    // tj. da li u svakoj metodi ponovo otvaraju kanal, i kad se zavrsi disposuju ga?

    public class HiringService : IHiringService
    {
        private HiringCompanyDB hiringCompanyDb = HiringCompanyDB.Instance();

        public void ResponseForPartnershipRequest(bool accepted, string outsourcingCompName)
        {

            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: HiringService.ResponseForPartnershipRequest(), " +
                                                  "params: bool accepted={0}, string outsorcingCompName={0}", accepted, outsourcingCompName));

            string notification = string.Empty;
            if (accepted)
            {
                hiringCompanyDb.PartnerCompaniesAddresses.Remove(outsourcingCompName.Trim()); // zasto brisemo? kako cemo onda kasnije da im posaljemo projekte ako im izbrisemo adresu?
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
            Program.Logger.Info(messageToLog);
        }

        public void ResponseForProjectRequest(string outsourcingCompanyName, ProjectCommon p)
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: HiringService.ResponseForProjectRequest(), " +
                                                  "params: string outsorcingCompName={0}, Project.Name={1}", outsourcingCompanyName,p.Name));

            string notification = string.Empty;

            Project proj = new Project();
            try
            {
                using (var access = new AccessDB())
                {
                    proj = access.Projects.SingleOrDefault(project => project.Name.Equals(p.Name));
                    if (proj != null)
                    {
                        if (p.IsAcceptedByOutsCompany)
                        {
                            proj.OutsourcingCompany = outsourcingCompanyName;
                            proj.IsAcceptedOutsCompany = true;
                            messageToLog.AppendLine("updated Project.IsAcceptedOutsCompanu data in .mdf database.");
                            access.SaveChanges();
                        }
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
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.AppendLine(string.Format("Method: HiringService.ResponseForProjectRequest(), " +
                                                  "params: Project.Name={0}, userStories.Count={1} ",  projectName,userStories.Count));


            string notification = string.Format("{0} User stories for project <{1}>, are waiting to be approved.",userStories.Count, projectName);
            Project proj = new Project();

            List<UserStory> tempUserStories = new List<UserStory>();

            foreach (UserStoryCommon us in userStories)
            {
                UserStory u = new UserStory(us.Title, us.Description, us.AcceptanceCriteria);
                tempUserStories.Add(u);
            }

            using (var access = new AccessDB())
            {
                proj = access.Projects.SingleOrDefault(project => project.Name.Equals(projectName));
                if (proj != null)
                {
                    proj.UserStories = tempUserStories;
                    access.SaveChanges();

                    messageToLog.AppendLine("Updated Project.UserStories data in .mdf database.");
                }

            }

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
                notifier.NotifySpecialClient(proj.ProductOwner, notification);
            }

        }
    }
}
