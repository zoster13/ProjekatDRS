using System;
using ClientCommon.Data;
using ICommon;
using System.ServiceModel;
using Server.Database;
using Server.Access;
using System.Collections.Generic;

namespace Server
{
    public class OutsourcingService : IOutsourcingService
    {
        public void AskForPartnership(string hiringCompanyName)
        {
            Notification notification = new Notification(NotificationType.REQUEST_FOR_PARTNERSHIP, hiringCompanyName, string.Empty, string.Empty);
            EmployeeService.Logger.Info("New notification for partnership request is arrived");

            Publisher.Instance.SendNotificationToCEO(notification);
        }

        public void SendEvaluatedUserstoriesToOutsourcingCompany(List<UserStoryCommon> userStories, string projectName)
        {
            EmployeeService.Logger.Info("Evaluated uset stories is arrived");
            Publisher.Instance.ReceiveUserStoriesCallback(userStories, projectName);
        }

        public void SendProjectToOutsourcingCompany(string hiringCompanyName, ProjectCommon p)
        {
            Notification notification = new Notification(NotificationType.PROJECT_REQUEST, hiringCompanyName, p.Name, p.Description);
            EmployeeService.Logger.Info("New notification for project request is arrived");

            Publisher.Instance.SendNotificationToCEO(notification);
        }
    }
}