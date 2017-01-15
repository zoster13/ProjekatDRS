﻿using System;
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
        //private IHiringCompanyDB hiringCompanyDb = HiringCompanyDB.Instance();
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

                //// bacati exc ako ime ne postoji
                internalDatabase.PartnerCompaniesAddresses.Add(outsourcingCompName.Trim(), internalDatabase.PossiblePartnersAddresses[outsourcingCompName]);
                internalDatabase.PossiblePartnersAddresses.Remove(outsourcingCompName.Trim());

                HiringCompanyDB.Instance.AddNewPartnerCompany(new PartnerCompany(outsourcingCompName)); // adding to db
                notification = "Company <" + outsourcingCompName + "> accepted request for partnership.";
            }
            else
            {
                //internalDatabase.ConnectionChannelsCompanies.Remove(outsourcingCompName); //Da li mi igde koristimo ovaj dictionary(s obzirom da pozivom svake metode ka drugom serveru uspostavljamo vezu)
               
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
           string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: HiringService.ResponseForProjectRequest(), " +
                                                  "params: string outsorcingCompName={0}, Project.Name={1}", outsourcingCompanyName, p.Name);
            Program.Logger.Info(messageToLog);
            //string notification = string.Empty;

            HiringCompanyDB.Instance.ResponseForProjectRequestFieldsChange(outsourcingCompanyName, p);

            //Project proj = new Project();
            //try
            //{
            //    using (var access = new AccessDB())
            //    {
            //        proj = access.Projects.SingleOrDefault(project => project.Name.Equals(p.Name));
            //        if (proj != null)
            //        {
            //            if (p.IsAcceptedByOutsCompany)
            //            {
            //                proj.OutsourcingCompany = outsourcingCompanyName;
            //                proj.IsAcceptedOutsCompany = true;
            //                access.SaveChanges();
            //                messageToLog = "updated Project.IsAcceptedOutsCompanu data in .mdf database.";
                           
            //                Program.Logger.Info(messageToLog);
            //            }
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

            //notification = p.IsAcceptedByOutsCompany ?
            //    string.Format("Company <" + outsourcingCompanyName + "> accepted request for developing project <" + p.Name + ">.") :
            //    string.Format("Company <" + outsourcingCompanyName + "> declined request for developing project <" + p.Name + ">.");


            //using (Notifier notifier = new Notifier())
            //{
            //    if (p.IsAcceptedByOutsCompany)
            //    {
            //        notifier.SyncAll();
            //    }
            //    notifier.NotifySpecialClient(proj.ProductOwner, notification);
            //    notifier.NotifySpecialClients(EmployeeType.CEO, notification);
            //}
        }

        public void SendUserStoriesToHiringCompany(List<UserStoryCommon> userStories, string projectName)
        {
           string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: HiringService.ResponseForProjectRequest(), " +
                                                  "params: Project.Name={0}, userStories.Count={1} ", projectName, userStories.Count);


            Program.Logger.Info(messageToLog);

            //string notification = string.Format("{0} User stories for project <{1}>, are waiting to be approved.", userStories.Count, projectName);
            //Project proj = new Project();

            List<UserStory> tempUserStories = new List<UserStory>();

            foreach (UserStoryCommon us in userStories)
            {
                UserStory u = new UserStory(us.Title, us.Description, us.AcceptanceCriteria);
                tempUserStories.Add(u);
            }

            HiringCompanyDB.Instance.SendUserStoriesToHiringCompanyFieldsChange(tempUserStories, projectName);
            //using (var access = new AccessDB())
            //{
            //    proj = access.Projects.SingleOrDefault(project => project.Name.Equals(projectName));
            //    if (proj != null)
            //    {
            //        proj.UserStories = tempUserStories;
            //        access.SaveChanges();

            //        messageToLog = "Updated Project.UserStories data in .mdf database.";
            //        Program.Logger.Info(messageToLog);
            //    }

            //}

            //using (Notifier notifier = new Notifier())
            //{
            //    notifier.SyncAll();
            //    notifier.NotifySpecialClient(proj.ProductOwner, notification);
            //}

        }


        public void SendClosedUserStory(string projectName, string title)
        {
           string messageToLog = string.Empty;
            messageToLog = string.Format("\nMethod: HiringService.SendClosedUserStory(), " +
                                                  "params: Project.Name={0}, UserStory.Title={1} ", projectName, title);
            Program.Logger.Info(messageToLog);

            HiringCompanyDB.Instance.SendClosedUserStoryFieldChange(projectName, title);

            //string notification = string.Empty;
            //string notification2 = string.Empty;

            //Project proj = new Project();
            //using (var access = new AccessDB())
            //{
            //    proj = access.Projects.Include("UserStories").SingleOrDefault(p => p.Name.Equals(projectName));
            //    if (proj != null)
            //    {
            //        UserStory us = proj.UserStories.Find(u => u.Title.Equals(title));
            //        us.IsClosed = true; 
            //        access.SaveChanges();
            //        notification = string.Format("User story <{0}> for project <{1}> is closed.", title, projectName);
            //        messageToLog = "Updated Project.UserStories data in .mdf database.";
            //        Program.Logger.Info(messageToLog);                   
            //    }
            //}

            //using (var access = new AccessDB())
            //{
            //    proj = access.Projects.Include("UserStories").SingleOrDefault(p => p.Name.Equals(projectName));
            //    List<UserStory> notClosedUserStories = proj.UserStories.FindAll(u => u.IsClosed == false); //ovo ne radi kod mene na kompu, sacuva mi u bazi da je u userStory
            //    if (notClosedUserStories.Count == 0 && proj.UserStories.Count != 0)
            //    {
            //        notification2 = string.Format("Project <{0}> can be closed, because all its user stories are closed.", projectName);
            //    }
            //}

            //using (Notifier notifier = new Notifier())
            //{
            //    notifier.SyncAll();               
            //    notifier.NotifySpecialClient(proj.ProductOwner, notification);
            //    if (!notification2.Equals(String.Empty))
            //    {
            //        notifier.NotifySpecialClient(proj.ProductOwner, notification2);
            //    }               
            //}
        }
    }
}
