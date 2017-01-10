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
    public class Task
    {
        private int id;
        private string title;
        private string description;
        private UserStory userStory;
        private AssignStatus assignStatus;
        private ProgressStatus progressStatus;
        private string employeeName;


        public Task()
        {
            assignStatus = AssignStatus.UNASSIGNED;
            progressStatus = ProgressStatus.INPREP;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
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
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [DataMember]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        [DataMember]
        public UserStory UserStory
        {
            get { return userStory; }
            set { userStory = value; }
        }

        [DataMember]
        public string ProjectName
        {
            get { return ProjectName; }
            set { ProjectName = value; }
        }

        [DataMember]
        public string EmployeeName
        {
            get { return employeeName; }
            set { employeeName = value; }
        }
    }
}
