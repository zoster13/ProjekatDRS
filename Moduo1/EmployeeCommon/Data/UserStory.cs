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
    public class UserStory
    {
        private int id; //kljuc u bazi
        private string title;
        private string description;
        private string acceptanceCriteria;
        private int storyPoints;

        public UserStory() 
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return id; }
            set { id = value; }
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
        public string AcceptanceCriteria
        {
            get { return acceptanceCriteria; }
            set { acceptanceCriteria = value; }
        }

        [DataMember]
        public int StoryPoints
        {
            get { return storyPoints; }
            set { storyPoints = value; }
        }
    }
}
