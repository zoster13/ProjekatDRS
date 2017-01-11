using System;
using ClientCommon.Data;
using ICommon;
using System.ServiceModel;
using Server.Database;
using Server.Access;

namespace Server
{
    public class OutsourcingService : IOutsourcingService
    {
        public void AskForPartnership(string hiringCompanyName)
        {
            Notification notification = new Notification(NotificationType.REQUEST_FOR_PARTNERSHIP, hiringCompanyName, string.Empty, string.Empty);

            Publisher.Instance.SendNotificationToCEO(notification);
        }

        public void SendProjectToOutsourcingCompany(string hiringCompanyName, ProjectCommon p)
        {
            Notification notification = new Notification(NotificationType.PROJECT_REQUEST, hiringCompanyName, p.Name, p.Description);

            Publisher.Instance.SendNotificationToCEO(notification);
        }


    }
}