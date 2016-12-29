using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon
{
    [DataContract]
    public class Project
    {
        private string name;
        private string description;
        private DateTime startDate;
        private DateTime deadline;
        private List<UserStory> userStories;
        private string outsourcingCompany;

        public Project() 
        {
            userStories=new List<UserStory>();
        }

        [DataMember]
        public string Name 
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [DataMember]
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        [DataMember]
        public DateTime Deadline
        {
            get { return deadline; }
            set { deadline = value; }
        }

        [DataMember]
        public List<UserStory> UserStories
        {
            get { return userStories; }
            set { userStories = value; }
        }

        [DataMember]
        public string OutsourcingCompany
        {
            get { return outsourcingCompany; }
            set { outsourcingCompany = value; }
        }

    }
}
