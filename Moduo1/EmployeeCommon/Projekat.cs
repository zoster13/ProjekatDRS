using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon
{
    public class Projekat
    {
        private string name;
        private string description;
        private DateTime startDate;
        private DateTime deadline;
        private List<UserStory> userStories;
        private string outsourcingCompany;

        public Projekat() 
        {
            userStories=new List<UserStory>();
        }

        public string Name 
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public DateTime Deadline
        {
            get { return deadline; }
            set { deadline = value; }
        }

        public List<UserStory> UserStories
        {
            get { return userStories; }
            set { userStories = value; }
        }

        public string OutsourcingCompany
        {
            get { return outsourcingCompany; }
            set { outsourcingCompany = value; }
        }

    }
}
