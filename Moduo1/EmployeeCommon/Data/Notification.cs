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
    public class Notification
    {
        private int id; // database key
        private string content;
        private string assignedUser_username;
        private string timestamp;

        public Notification()
        {

        }

        public Notification(string assignedUser_username, string timestamp)
        {
            this.assignedUser_username = assignedUser_username;
            this.timestamp = timestamp;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        [DataMember]
        public string Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

    }
}
