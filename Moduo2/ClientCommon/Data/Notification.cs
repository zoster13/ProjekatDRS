using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

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
        
        public Notification()
        {
            notificationStamp = DateTime.Now;
            status = NotificationStatus.PENDING;
        }

        public Notification(NotificationType type, string hiringCompany, string projectName)
        {
            this.type = type;
            notificationStamp = DateTime.Now;
            status = NotificationStatus.PENDING;

            switch (type)
            {
                case NotificationType.REQUEST_FOR_PARTNERSHIP:
                    message = "Hiring company: " + hiringCompany + " has asked for your partnership. \nDo you accept or decline?";
                    break;
                case NotificationType.PROJECT_REQUEST:
                    message = "Hiring company: " + hiringCompany + " has sent you a project.\n Do you accept or decline?";
                    break;
            }
        }

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
