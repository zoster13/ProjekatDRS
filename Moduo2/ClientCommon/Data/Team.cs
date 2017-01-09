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
        private Employee teamLeader;
        private Employee scrumMaster;

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
        public Employee TeamLeader
        {
            get { return teamLeader; }
            set { teamLeader = value; }
        }
        
        [DataMember]
        public Employee ScrumMaster
        {
            get { return scrumMaster; }
            set { scrumMaster = value; }
        }

        [DataMember]
        public List<Project> Projects
        {
            get { return projects; }
            set { projects = value; }
        }

        public override string ToString()
        {
            if(teamLeader != null && scrumMaster != null)
            {
                return Name + ", " + TeamLeader.Name + " " + TeamLeader.Surname + ", " + scrumMaster.Name + " " + scrumMaster.Surname;
            }
            else if (teamLeader == null && scrumMaster == null)
            {
                return Name + ", no team leader and no scrum master";
            }
            else if(teamLeader == null && scrumMaster != null)
            {
                return Name + ", no team leader , " + scrumMaster.Name + " " + scrumMaster.Surname;
            }
            else
            {
                return Name + ", " + TeamLeader.Name + " " + TeamLeader.Surname + ", no scrum master";
            }
        }
    }
}
