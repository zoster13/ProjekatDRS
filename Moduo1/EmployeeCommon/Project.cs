using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon
{
    [DataContract]
    public class Project
    {
        private int id; //kljuc u bazi
        private string name;
        private string description;
        private DateTime startDate;
        private DateTime deadline;
        private List<UserStory> userStories;
        private string outsourcingCompany;
        private string productOwner;

        public Project() 
        {
            userStories=new List<UserStory>();
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
    }
}
