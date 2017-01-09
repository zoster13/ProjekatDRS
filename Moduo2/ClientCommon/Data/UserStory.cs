﻿using System;
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
        private ProgressStatus progressStatus;
        private AcceptStatus acceptStatus;
        private DateTime deadline;

        private List<Task> tasks;

        public UserStory()
        {
            acceptStatus = AcceptStatus.PENDING;
            progressStatus = ProgressStatus.INPREP;
            deadline = new DateTime();
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

        [DataMember]
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
    }
}