using EmployeeCommon.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon.Data
{
    [DataContract]
    public class Employee 
    {
        private string username;
        private string password;
        private EmployeeType type;
        private string name;
        private string surname;
        private string email;
        private int startHour;
        private int startMinute;
        private int endHour;
        private int endMinute;
        private List<Notification> notifications;

        public Employee() 
        {

        }

        public Employee(string eUsername, string ePassword, EmployeeType eType, string eName, string eSurname, string eEmail,int eStartHour,int eStartMinute,int eEndHour,int eEndMinute) 
        {
            username = eUsername;
            password = ePassword;
            type = eType;
            name = eName;
            surname = eSurname;
            email = eEmail;
            startHour = eStartHour; // napraviti proveru opsega 0-24h, 0-59min
            startMinute = eStartMinute;
            endHour = eEndHour;
            endMinute = eEndMinute;
            notifications = new List<Notification>();
        }

        [DataMember]
        [Required]
        [Key]
        public string Username 
        {
            get { return username; }
            set { username = value; }
        }

        [DataMember]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [DataMember]
        public EmployeeType Type
        {
            get { return type; }
            set { type = value; }
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }

        [DataMember]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [DataMember]
        public int StartHour 
        {
            get { return startHour; }
            set { startHour = value; }
        }

        [DataMember]
        public int StartMinute
        {
            get { return startMinute; }
            set { startMinute = value; }
        }

        [DataMember]
        public int EndHour
        {
            get { return endHour; }
            set { endHour = value; }
        }

        [DataMember]
        public int EndMinute
        {
            get { return endMinute; }
            set { endMinute = value; }
        }

        [DataMember]
        public List<Notification> Notifications
        {
            get { return notifications; }
            set { notifications = value; }
        }
    }
}
