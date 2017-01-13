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
    public class UserStory
    {
        private int id;
        private string title;
        private string description;
        private int difficulty;
        private string acceptanceCriteria;
        private ProgressStatus progressStatus;
        private AcceptStatus acceptStatus;
        private DateTime deadline;
        private Project project = null;

        private List<Task> tasks;

        public UserStory()
        {
            acceptStatus = AcceptStatus.PENDING;
            progressStatus = ProgressStatus.INPREP;
            deadline = DateTime.Now;
            title = string.Empty;
            description = string.Empty;
            acceptanceCriteria = string.Empty;
            difficulty = 2;
            project = new Project();
            tasks = new List<Task>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
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
        public int Difficulty
        {
            get { return difficulty; }
            set { difficulty = value; }
        }

        [DataMember]
        public string AcceptanceCriteria
        {
            get { return acceptanceCriteria; }
            set { acceptanceCriteria = value; }
        }

        [DataMember]
        public AcceptStatus AcceptStatus
        {
            get { return acceptStatus; }
            set { acceptStatus = value; }
        }

        [DataMember]
        public ProgressStatus ProgressStatus
        {
            get { return progressStatus; }
            set { progressStatus = value; }
        }

        [IgnoreDataMember]
        public List<Task> Tasks
        {
            get { return tasks; }
            set { tasks = value; }
        }

        [DataMember]
        public DateTime Deadline
        {
            get { return deadline; }
            set { deadline = value; }
        }

        [DataMember]
        public Project Project
        {
            get { return project; }
            set { project = value; }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
