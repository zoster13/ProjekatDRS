using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClientCommon.Data
{
    [DataContract]
    public class Notification
    {
        protected int id;
        protected NotificationType type;
        protected DateTime notificationStamp;
        protected NotificationStatus status;
        protected string message;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        [DataMember]
        public NotificationType Type
        {
            get { return type; }
            set { type = value; }
        }

        [DataMember]
        public DateTime NotificationStamp
        {
            get { return notificationStamp; }
            set { notificationStamp = value; }
        }

        [DataMember]
        public NotificationStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
