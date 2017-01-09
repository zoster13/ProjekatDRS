using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ICommon
{
    [DataContract]
    public class UserStoryCommon
    {
        private string title;
        private string description;
        private string acceptanceCriteria;
        private bool isAccepted;

        public UserStoryCommon() 
        {
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
        public bool IsAccepted 
        {
            get { return isAccepted; }
            set { isAccepted = value; }
        }
    }
}
