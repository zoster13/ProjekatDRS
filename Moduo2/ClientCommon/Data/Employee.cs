using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

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
        private Team team;
        private DateTime passwordTimeStamp;
        private DateTime workingHoursStart;
        private DateTime workingHoursEnd;
        private ICallbackMethods channel;

        private List<Notification> notifications;
        
        public Employee()
        {
            type = EmployeeType.DEVELOPER;
            name = string.Empty;
            surname = string.Empty;
            email = string.Empty;
            password = string.Empty;
            team = new Team();
            passwordTimeStamp = DateTime.Now;
            workingHoursStart = DateTime.Now;
            workingHoursEnd = DateTime.Now;
            notifications = new List<Notification>();
        }

        public Employee(EmployeeType type, string name, string surname, string email, string password, Team team)
        {
            this.type = type;
            this.name = name;
            this.surname = surname;
            this.email = email;
            this.password = password;
            this.team = team;
            this.workingHoursStart = DateTime.Now;
            this.workingHoursEnd = DateTime.Now;
            this.passwordTimeStamp = DateTime.Now;
            this.channel = null;
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
        public Team Team
        {
            get { return this.team; }
            set { this.team = value; }
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
        
        [IgnoreDataMember]
        public ICallbackMethods Channel
        {
            get { return this.channel; }
            set { this.channel = value; }
        }
        
        public override string ToString()
        {
            return Name + " " + Surname;
        }
    }
}
