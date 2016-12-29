using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
namespace EmployeeCommon
{
    [DataContract]
    public class Notification
    {
        /// <summary>
        /// Type of client to be notified
        /// </summary>
        private EmployeeType destination;

        /// <summary>
        /// Speciffic client type related notifType 
        /// </summary>
        private NotificationType notifType;

        [DataMember]
        public EmployeeType Destination
        {
            get { return destination; }
            set { destination = Destination; }
        }

        [DataMember]
        public NotificationType NotifType
        {
            get { return notifType; }
            set { notifType=value;  }  
        }
    }
}
