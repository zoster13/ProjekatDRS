using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ClientCommon.Data
{
    [DataContract]
    public class Employee
    {
        private int id;
        private EmployeeType type;
        private string name;
        private string surname;
        private string email;
        private string password;
        private string teamName;
        private DateTime passwordTimeStamp;
        private DateTime workingHoursStart;
        private DateTime workingHoursEnd;

        private List<Notification> notifications;

        private ICollection<Team> team;

        public Employee()
        {
            team = new List<Team>();
            passwordTimeStamp = new DateTime();
            workingHoursStart = new DateTime();
            workingHoursEnd = new DateTime();
            notifications = new List<Notification>();
        }

        public Employee(EmployeeType type, string name, string surname, string email, string password, string teamName)
        {
            this.type = type;
            this.name = name;
            this.surname = surname;
            this.email = email;
            this.password = password;
            this.teamName = teamName;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        [DataMember]
        public EmployeeType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        [DataMember]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        [DataMember]
        public string Surname
        {
            get { return this.surname; }
            set { this.surname = value; }
        }

        [DataMember]
        public string Email
        {
            get { return this.email; }
            set { this.email = value; }
        }

        [DataMember]
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        [DataMember]
        public string TeamName
        {
            get { return this.teamName; }
            set { this.teamName = value; }
        }

        [DataMember]
        public DateTime PasswordTimeStamp
        {
            get { return passwordTimeStamp; }
            set { passwordTimeStamp = value; }
        }

        [DataMember]
        public DateTime WorkingHoursStart
        {
            get { return workingHoursStart; }
            set { workingHoursStart = value; }
        }

        [DataMember]
        public DateTime WorkingHoursEnd
        {
            get { return workingHoursEnd; }
            set { workingHoursEnd = value; }
        }

        [DataMember]
        public List<Notification> Notifications
        {
            get { return notifications; }
            set { notifications = value; }
        }

        [DataMember]
        public ICollection<Team> Team { get; set; }

        public override string ToString()
        {
            return Name + " " + Surname;
        }
    }
}
