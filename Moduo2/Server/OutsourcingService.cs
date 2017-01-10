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
    }
}
