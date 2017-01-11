using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ICommon
{
    [DataContract]
    public class ProjectCommon
    {
        private string name;
        private string description;
        private DateTime startDate;
        private DateTime deadline;
        private bool isAcceptedByOutsCompany;

        public ProjectCommon() 
        {
            isAcceptedByOutsCompany = false;
        }

        public ProjectCommon(string n, string desc, DateTime startD, DateTime dead) 
        {
            name = n;
            description = desc;
            startDate = startD;
            deadline = dead;
            isAcceptedByOutsCompany = false;
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
        public bool IsAcceptedByOutsCompany 
        {
            get { return isAcceptedByOutsCompany; }
            set { isAcceptedByOutsCompany = value; }
        }
    }
}
