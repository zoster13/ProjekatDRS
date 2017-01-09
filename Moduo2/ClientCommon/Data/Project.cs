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
        private string teamName;
        private AssignStatus assignStatus;
        private ProgressStatus progressStatus;
        private Team team;

        private List<UserStory> userStories;

        public Project()
        {
            userStories = new List<UserStory>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        [DataMember]
        public string TeamName
        {
            get { return teamName; }
            set { teamName = value; }
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
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

        [DataMember]
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
