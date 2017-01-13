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
    public class UserStory
    {
        private int id; // database key
        private string title;
        private string description;
        private string acceptanceCriteria;
        private bool isApprovedByPO;
        private bool isClosed;

        public UserStory() 
        {
            isApprovedByPO = false;
            isClosed = false;
        }

        public UserStory(string title, string description, string acceptanceCriteria) 
        {
            this.title = title;
            this.description = description;
            this.acceptanceCriteria = acceptanceCriteria;
            isApprovedByPO = false;
            isClosed = false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Title 
        {
            get { return title; }
            set { title = value; }
        }

        [DataMember]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [DataMember]
        public string AcceptanceCriteria
        {
            get { return acceptanceCriteria; }
            set { acceptanceCriteria = value; }
        }

        [DataMember]
        public bool IsApprovedByPO
        {
            get { return isApprovedByPO; }
            set { isApprovedByPO = value; }
        }

        [DataMember]
        public bool IsClosed 
        {
            get { return isClosed; }
            set { isClosed = value; }
        }
    }
}
