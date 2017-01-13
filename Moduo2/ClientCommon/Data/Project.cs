using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClientCommon.Data
{
    [DataContract]
    public class Project
    {
        private int id;
        private string name;
        private string description;
        private AssignStatus assignStatus;
        private ProgressStatus progressStatus;
        private Team team;
        private string hiringCompanyName;

        private List<UserStory> userStories;

        public Project()
        {
            name = string.Empty;
            description = string.Empty;
            hiringCompanyName = string.Empty;
            userStories = new List<UserStory>();
            assignStatus = AssignStatus.UNASSIGNED;
            progressStatus = ProgressStatus.INPREP;
            team = new Team();
        }

        public Project(string projName, string hiringCompanyName)
        {
            userStories = new List<UserStory>();
            assignStatus = AssignStatus.UNASSIGNED;
            progressStatus = ProgressStatus.INPREP;
            this.hiringCompanyName = hiringCompanyName;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
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
        public string HiringCompanyName
        {
            get { return hiringCompanyName; }
            set { hiringCompanyName = value; }
        }
        
        [DataMember]
        public AssignStatus AssignStatus
        {
            get { return assignStatus; }
            set { assignStatus = value; }
        }

        [DataMember]
        public ProgressStatus ProgressStatus
        {
            get { return progressStatus; }
            set { progressStatus = value; }
        }

        [IgnoreDataMember]
        public List<UserStory> UserStories
        {
            get { return userStories; }
            set { userStories = value; }
        }

        [DataMember]
        public Team Team
        {
            get { return team; }
            set { team = value; }
        }



        public override string ToString()
        {
            return name + ", " + assignStatus;
        }
    }
}
