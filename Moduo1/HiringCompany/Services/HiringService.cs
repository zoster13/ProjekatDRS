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

           string messageToLog=string.Empty;
            messageToLog=(string.Format("Method: HiringService.ResponseForPartnershipRequest(), " +
                                                  "params: bool accepted={0}, string outsorcingCompName={0}", accepted, outsourcingCompName));
            Program.Logger.Info(messageToLog);

            string notification = string.Empty;
            if (accepted)
            {
                //hiringCompanyDb.PartnerCompaniesAddresses.Remove(outsourcingCompName.Trim()); // zasto brisemo? kako cemo onda kasnije da im posaljemo projekte ako im izbrisemo adresu?
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
        }

        public void ResponseForProjectRequest(string outsourcingCompanyName, ProjectCommon p)
        {
           string messageToLog=string.Empty;
            messageToLog=(string.Format("Method: HiringService.ResponseForProjectRequest(), " +
                                                  "params: string outsorcingCompName={0}, Project.Name={1}", outsourcingCompanyName,p.Name));
            Program.Logger.Info(messageToLog);
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
                            access.SaveChanges();
                            messageToLog=("updated Project.IsAcceptedOutsCompanu data in .mdf database.");
                           
                            Program.Logger.Info(messageToLog);
                        }
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageToLog=(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    Program.Logger.Info(messageToLog);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageToLog=(string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                        Program.Logger.Info(messageToLog);
                    }
                }
            }

            notification = p.IsAcceptedByOutsCompany ?
                string.Format("Company <" + outsourcingCompanyName + "> accepted request for developing project <" + p.Name + ">.") :
                string.Format("Company <" + outsourcingCompanyName + "> declined request for developing project <" + p.Name + ">.");


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
           string messageToLog=string.Empty;
            messageToLog=(string.Format("Method: HiringService.ResponseForProjectRequest(), " +
                                                  "params: Project.Name={0}, userStories.Count={1} ",  projectName,userStories.Count));


            Program.Logger.Info(messageToLog);

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

                    messageToLog=("Updated Project.UserStories data in .mdf database.");
                    Program.Logger.Info(messageToLog);
                }

            }

            using (Notifier notifier = new Notifier())
            {
                notifier.SyncAll();
                notifier.NotifySpecialClient(proj.ProductOwner, notification);
            }

        }


        public void SendClosedUserStory(string projectName, string title)
        {

            // napraviti onu proveru da li su svi user storiji zatvoreni da bi znali koje projekte da zatvaramo
           string messageToLog=string.Empty;
            messageToLog=(string.Format("Method: HiringService.SendClosedUserStory(), " +
                                                  "params: Project.Name={0}, UserStory.Title={1} ", projectName, title));
            Program.Logger.Info(messageToLog);

            string notification = string.Format("User story <{0}> for project <{1}> is closed.", title, projectName);

            Project proj = new Project();
            using (var access = new AccessDB())
            {
                proj = access.Projects.Include("UserStories").SingleOrDefault(p => p.Name.Equals(projectName));
                if (proj != null)
                {
                    UserStory us=proj.UserStories.Find(u => u.Title.Equals(title));
                    us.IsClosed = true;
                    access.SaveChanges();

                    messageToLog=("Updated Project.UserStories data in .mdf database.");
                    Program.Logger.Info(messageToLog);
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
