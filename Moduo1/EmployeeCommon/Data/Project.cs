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
    public class Project
    {
        private int id; // database key
        private string name;
        private string description;
        private DateTime startDate;
        private DateTime deadline;
        private List<UserStory> userStories;
        private string outsourcingCompany;
        private string productOwner;
        private string scrumMaster;
        private bool isAcceptedCEO;
        private bool isAcceptedOutsCompany;

        public Project() 
        {
            userStories=new List<UserStory>();
            isAcceptedCEO = false;
            isAcceptedOutsCompany = false;
        }

        public Project(string name, string description, string po, string sm)
        {
            this.name = name;
            this.description = description;
            this.productOwner = po;
            this.scrumMaster = sm;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id 
        {
            get { return id; }
            set { id = value; }
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

        [DataMember]
        public string ProductOwner
        {
            get { return productOwner; }
            set { productOwner = value; }
        }

        [DataMember]
        public string ScrumMaster
        {
            get { return scrumMaster; }
            set { scrumMaster = value; }
        }

        [DataMember]
        public bool IsAcceptedCEO 
        {
            get { return isAcceptedCEO; }
            set { isAcceptedCEO = value; }
        }

        [DataMember]
        public bool IsAcceptedOutsCompany 
        {
            get { return isAcceptedOutsCompany; }
            set { isAcceptedOutsCompany = value; }
        }
    }
}
