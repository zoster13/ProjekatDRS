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
            projects = new List<Project>();
        }

        public Team(string name)
        {
            this.name = name;
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

        [DataMember]
        public List<Project> Projects
        {
            get { return projects; }
            set { projects = value; }
        }

        public override string ToString()
        {
            return Name;

            //if(teamLeaderEmail.Equals(string.Empty) && scrumMasterEmail.Equals(string.Empty))
            //{
            //    return Name + ", TL: " + TeamLeaderEmail + ", SM: " + scrumMasterEmail;
            //}
            //else if (teamLeaderEmail == null && scrumMaster == null)
            //{
            //    return Name + ", no team leader and no scrum master";
            //}
            //else if(teamLeader == null && scrumMaster != null)
            //{
            //    return Name + ", no team leader , " + scrumMaster.Name + " " + scrumMaster.Surname;
            //}
            //else
            //{
            //    return Name + ", " + TeamLeader.Name + " " + TeamLeader.Surname + ", no scrum master";
            //}
        }
    }
}
