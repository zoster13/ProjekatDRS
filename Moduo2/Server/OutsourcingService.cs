using System;
using ClientCommon.Data;
using ICommon;

namespace Server
{
    public class OutsourcingService : IOutsourcingService
    {
        public void AskForPartnership(string hiringCompanyName)
        {
            Notification notification = new Notification(NotificationType.REQUEST_FOR_PARTNERSHIP,hiringCompanyName, string.Empty);

            Publisher.Instance.SendNotificationToCEO(notification);
        }
    }
}
