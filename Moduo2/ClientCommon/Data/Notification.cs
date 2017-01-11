using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ICommon;
using System.Collections.Generic;

namespace ClientCommon.Data
{
    [DataContract]
    public class Notification
    {
        protected int id;
        protected NotificationType type;
        protected DateTime notificationStamp;
        protected NotificationAcceptStatus statusAccept;
        protected NotificationNewStatus statusNew;
        protected string message;
        private string hiringCompanyName;
        private string projectName;
        private string projectDescription;
        private Employee employee;

        public Notification()
        {
            notificationStamp = DateTime.Now;
            statusAccept = NotificationAcceptStatus.PENDING;
        }

        public Notification(NotificationType type, string hiringCompany, string projectName, string projectDescription)
        {
            this.type = type;
            notificationStamp = DateTime.Now;
            statusAccept = NotificationAcceptStatus.PENDING;
            statusNew = NotificationNewStatus.NEW;
            hiringCompanyName = hiringCompany;
            this.projectName = projectName;
            this.projectDescription = projectDescription;

            switch (type)
            {
                case NotificationType.REQUEST_FOR_PARTNERSHIP:
                    message = "Hiring company: " + hiringCompany + " has asked for your partnership. \nDo you accept or decline?";
                    break;
                case NotificationType.PROJECT_REQUEST:
                    message = "Hiring company: " + hiringCompany + " has sent you a project named: " + projectName + ".\n Do you accept or decline?";
                    break;
                case NotificationType.NEW_PROJECT_TL:
                    statusAccept = NotificationAcceptStatus.NO_STATUS;
                    message = "Your team has been assigned a new project: " + projectName + ". Write user stories for it and send for evaluation.";
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
        public NotificationAcceptStatus StatusAccept
        {
            get { return statusAccept; }
            set { statusAccept = value; }
        }

        [DataMember]
        public NotificationNewStatus StatusNew
        {
            get { return statusNew; }
            set { statusNew = value; }
        }

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        [DataMember]
        public string HiringCompanyName
        {
            get { return hiringCompanyName; }
            set { hiringCompanyName = value; }
        }

        [DataMember]
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }

        [DataMember]
        public string ProjectDescription
        {
            get { return projectDescription; }
            set { projectDescription = value; }
        }

        [IgnoreDataMember]
        public Employee Emoloyee
        {
            get { return employee; }
            set { employee = value; }
        }

    }
}
