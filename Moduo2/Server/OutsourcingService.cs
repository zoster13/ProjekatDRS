using System;
using ClientCommon.Data;
using ICommon;
using System.ServiceModel;

namespace Server
{
    public class OutsourcingService : IOutsourcingService
    {
        IOutsourcingServiceCallback outsourcingCallbackChannel;

        public void AskForPartnership(string hiringCompanyName)
        {
            outsourcingCallbackChannel = OperationContext.Current.GetCallbackChannel<IOutsourcingServiceCallback>();
            Notification notification = new Notification(NotificationType.REQUEST_FOR_PARTNERSHIP,hiringCompanyName, string.Empty);

            Publisher.Instance.SendNotificationToCEO(notification, outsourcingCallbackChannel);
        }

        public void SendProjectToOutsourcingCompany(string hiringCompanyName, ProjectCommon p)
        {
            //Project newProject = new Project(hiringCompanyName, p.Name);
            //Publisher.Instance.SendProjectToCEO(hiringCompanyName, newProject);

            Notification notification = new Notification(NotificationType.PROJECT_REQUEST, hiringCompanyName, p.Name);
            Publisher.Instance.SendNotificationToCEO(notification, outsourcingCallbackChannel);
        }
    }
}