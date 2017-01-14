using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ClientCommon.Data
{
    [DataContract]
    public class Team
    {
        private int id;
        private string name;
        private string teamLeaderEmail;
        private string scrumMasterEmail;

        private List<Project> projects;

        public Team()
        {
            name = string.Empty;
            teamLeaderEmail = string.Empty;
            scrumMasterEmail = string.Empty;
            projects = new List<Project>();
        }

        public Team(string name)
        {
            this.name = name;
            teamLeaderEmail = string.Empty;
            scrumMasterEmail = string.Empty;
            projects = new List<Project>();
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
            get { return this.name; }
            set { this.name = value; }
        }

        [DataMember]
        public string TeamLeaderEmail
        {
            get { return teamLeaderEmail; }
            set { teamLeaderEmail = value; }
        }
        
        [DataMember]
        public string ScrumMasterEmail
        {
            get { return scrumMasterEmail; }
            set { scrumMasterEmail = value; }
        }

        [IgnoreDataMember]
        public List<Project> Projects
        {
            get { return projects; }
            set { projects = value; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
