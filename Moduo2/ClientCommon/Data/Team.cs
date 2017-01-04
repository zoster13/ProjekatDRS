using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCommon.Data
{
    public class Team
    {
        private int id;
        private string name;
        private Employee teamLeader;
        //List<Employee> employees;

        public Team()
        {

        }

        public Team(string name)
        {
            this.name = name;
            //this.employees = new List<Data.Employee>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public Employee TeamLeader
        {
            get { return teamLeader; }
            set { teamLeader = value; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
